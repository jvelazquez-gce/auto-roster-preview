using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;

namespace Services.OneStudentPerSection
{
    public class OneStudentPerSectionCVueSectionsHandler : IOneStudentPerSectionCVueSectionsHandler
    {
        private readonly ICalculator _calculator;

        public OneStudentPerSectionCVueSectionsHandler(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public OneStudentPerSectionResults AddUpdateAndInactivateCampusVueSectionsToCancel(Parameters p)
        {
            var sectionAdClassSchedIDsFromCVueToCancel = p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue
                                                                    .Select(s => s.AdClassSchedID)
                                                                    .Distinct()
                                                                    .ToList();

            var results = InactivateAndDeleteCVueSectionsToBeCancelAndGetDeletedSectionsThatNeedToBeReAdded(p, sectionAdClassSchedIDsFromCVueToCancel);

            results.CreatedSectionsToCancel = CreateCourseSectionsToCancel(p, results.SectionAdClassSchedIDsFromCVueToCancel);

            return results;
        }

        public OneStudentPerSectionResults InactivateAndDeleteCVueSectionsToBeCancelAndGetDeletedSectionsThatNeedToBeReAdded(Parameters p, List<int> SectionAdClassSchedIDsFromCVueToCancel)
        {
            var courseSectionsToDeleteOrInactivate = p.GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery.ExecuteQuery(p.QueryInputParams);

            // TODO: update this code to not delete and re-add classes that need to be cancelled
            var sectionAdClassSchedIDsFromCVueToCancelThatAlreadyExist = new List<int>();
            var results = new OneStudentPerSectionResults();
            foreach (var record in courseSectionsToDeleteOrInactivate)
            {
                var adClassSchedId = (int)record.AdClassSchedId;
                if (SectionAdClassSchedIDsFromCVueToCancel.Contains(adClassSchedId))
                {
                    sectionAdClassSchedIDsFromCVueToCancelThatAlreadyExist.Add(adClassSchedId);
                }
                else
                {
                    record.Active = false;
                    record.StatusID = SectionStatus.INACTIVE;
                    record.IsManagedByARB = false;
                    record.HasSectionBeenCreatedOrUpdatedInCampusVue = false;
                    record.UpdatedBy = GeneralStatus.ARB_JOB;
                    record.UpdatedOn = DateTime.Now;
                    // TODO: need to test this
                    p.UpdateCourseSectionWithoutSavingCommand.ExecuteCommand(record);
                    results.CourseSectionsToDeleteOrInactivate.Add(record);
                }
            }
            sectionAdClassSchedIDsFromCVueToCancelThatAlreadyExist.ForEach(r => SectionAdClassSchedIDsFromCVueToCancel.Remove(r));
            results.SectionAdClassSchedIDsFromCVueToCancelThatAlreadyExist = sectionAdClassSchedIDsFromCVueToCancelThatAlreadyExist;
            results.SectionAdClassSchedIDsFromCVueToCancel = SectionAdClassSchedIDsFromCVueToCancel;

            p.SaveChangesCommand.ExecuteCommand();
            return results;
        }

        public List<CourseSection> CreateCourseSectionsToCancel(Parameters p, List<int> SectionAdClassSchedIDsFromCVueToAddToDbToCancel)
        {
            var courseSectionsToCancel = new List<CourseSection>();
            if (!SectionAdClassSchedIDsFromCVueToAddToDbToCancel.Any()) return courseSectionsToCancel;

            foreach (var adClassSchedId in SectionAdClassSchedIDsFromCVueToAddToDbToCancel)
            {
                var firstRecord = p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue
                    .FirstOrDefault(r => r.AdClassSchedID == adClassSchedId);

                if (firstRecord == null) throw new Exception("Invalid data mismatch with campusvue sections to cancel with AdClassSchedId: " + adClassSchedId);
                var guid = Guid.NewGuid();
                var sectionToCancel = new CourseSection()
                {
                    Id = guid,
                    JobID = p.Job.Id,
                    CampusCode = GeneralStatus.CAMPUS_ONLINE,
                    CourseCode = firstRecord.CourseCode,
                    AdCourseID = p.CalcModel.CourseID,
                    AdTermID = firstRecord.AdTermID,
                    StartDate = p.CalcModel.StartDate,
                    ExternalSystem = GeneralStatus.ARB_JOB,
                    ExternalSystemId = guid.ToString(),
                    TermCode = firstRecord.Term,
                    MaximumNumberOfStudents = _calculator.GetMaxNumberOfStudentsPerSection(firstRecord.TargetStudentCount, firstRecord.GroupNumber, firstRecord.GroupTargetStudentCount),
                    PassFailSetting = GeneralStatus.PassFailType.NoOptionForPassFail,
                    PrimaryInstructor = firstRecord.PrimaryInstructor,
                    SectionCode = firstRecord == null ? string.Empty : firstRecord.SectionCode,
                    SectionDeliveryMethod = GeneralStatus.ONLINE_DELIVERY_METHOD,
                    HasInstructor = firstRecord == null ? false : firstRecord.HasInstructor,
                    HasSectionBeenCreatedOrUpdatedInCampusVue = false,
                    GroupCategory = ARBGroupCategory.ONE_STUDENT_PER_SECTION,
                    StatusID = SectionStatus.READY_TO_CANCEL_SECTION_IN_CVUE,
                    Active = false,
                    LmsExtractStatus = firstRecord.LmsExtractStatus,
                    SyLmsVendorID = firstRecord.SysLmsVendorID,
                    EndDate = firstRecord.EndDate,
                    AdClassSchedId = adClassSchedId,
                    OriginalCVueMaximumNumberOfStudents = firstRecord.CVueMaxStudents,
                    AdTeacherID = firstRecord.AdTeacherID,
                    InstructorAssignedStatusID = firstRecord.InstructorAssignedStatusID,
                    ReasonToUnregisterStudents = GeneralStatus.REASON_1_TO_UNREGISTER_STUDENTS,
                    CreatedBy = GeneralStatus.ARB_JOB,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = GeneralStatus.ARB_JOB,
                    UpdatedOn = DateTime.Now
                };
                courseSectionsToCancel.Add(sectionToCancel);
            }
            return p.AddCourseSectionsCommand.ExecuteCommand(courseSectionsToCancel);
        }
    }
}
