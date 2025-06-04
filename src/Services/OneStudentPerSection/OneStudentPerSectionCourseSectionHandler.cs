using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.Utilities;

namespace Services.OneStudentPerSection
{
    public class OneStudentPerSectionCourseSectionHandler : IOneStudentPerSectionCourseSectionHandler
    {
        private readonly IConfigInstance _configInstance;
        private readonly IOneStudentPerSectionErrorChecking _oneStudentPerSectionErrorChecking;

        public OneStudentPerSectionCourseSectionHandler(IConfigInstance configInstance, IOneStudentPerSectionErrorChecking oneStudentPerSectionErrorChecking) 
        {
            _configInstance = configInstance;
            _oneStudentPerSectionErrorChecking = oneStudentPerSectionErrorChecking;
        }

        public OneStudentPerSectionResults ExecuteCourseSectionLogic(Parameters p, IConfigurationRepository configurationRepository, OneStudentPerSectionResults results)
        {
            List<OneStudentPerSectionPair> OneStudentPerSectionPairListToUpdate = new List<OneStudentPerSectionPair>();
            List<OneStudentPerSectionPair> OneStudentPerSectionPairListWithErrors = new List<OneStudentPerSectionPair>();

            InactivateSectionsWhereStudentsDropdOffAndGetRecordsWithErrors(p, OneStudentPerSectionPairListToUpdate, OneStudentPerSectionPairListWithErrors);
            HandleErrorInactivationRecords(OneStudentPerSectionPairListWithErrors, p);
            results.NewlyPairRecordsSaveToDbToBeCreatedAndTransfer = CreateCourseSectionsToAddToCampusVueAndSaveSectionsAndStudentRecordsToDb(p, configurationRepository);

            return results;
            // need to test if classes with instructors can be cancelled
            // new OneStudentPerSectionCVueSectionsHandler().AddUpdateAndInactivateCampusVueSectionsToCancel(p);
        }
        
        public void InactivateSectionsWhereStudentsDropdOffAndGetRecordsWithErrors(Parameters p, List<OneStudentPerSectionPair> OneStudentPerSectionPairListToUpdate, 
            List<OneStudentPerSectionPair> OneStudentPerSectionPairListWithErrors)
        {
            foreach (var studentRecord in p.ListOfRecordsFromDbMissingMatchingRecordFromCVue)
            { 
                var courseSections = p.OneStudentPerSectionCourseSectionsReadyToCreateOrCreated
                                        .Where(r => r.Id == studentRecord.SectionGuid
                                            && r.StatusID == SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE).ToList();

                var oneStudentPerSectionPair = new OneStudentPerSectionPair();
                if (courseSections == null || courseSections.Count == 0)
                {
                    oneStudentPerSectionPair.StatusID = OneStudentPerSectionStatus.STUDENT_RECORD_WITH_MISSING_COURSE_SECTION;
                    studentRecord.StatusID = OneStudentPerSectionStatus.STUDENT_RECORD_WITH_MISSING_COURSE_SECTION;
                    studentRecord.UpdatedOn = DateTime.Now;
                    oneStudentPerSectionPair.OneStudentPerSectionList.Add(studentRecord);

                    OneStudentPerSectionPairListWithErrors.Add(oneStudentPerSectionPair);
                }
                else if (courseSections != null && courseSections.Count > 1)
                {
                    oneStudentPerSectionPair.StatusID = OneStudentPerSectionStatus.STUDENT_RECORD_WITH_MORE_THAN_ONE_COURSE_SECTION;
                    studentRecord.StatusID = OneStudentPerSectionStatus.STUDENT_RECORD_WITH_MORE_THAN_ONE_COURSE_SECTION;
                    studentRecord.UpdatedOn = DateTime.Now;
                    oneStudentPerSectionPair.OneStudentPerSectionList.Add(studentRecord);

                    foreach(var courseSection in courseSections)
                        oneStudentPerSectionPair.CourseSections.Add(SetSectionToInactive(courseSection));

                    OneStudentPerSectionPairListWithErrors.Add(oneStudentPerSectionPair);
                }
                else if (courseSections != null && courseSections.Count == 1)
                {
                    var courseSection = courseSections.First();

                    studentRecord.StatusID = StudentSectionStatus.CANCELLED_DUE_TO_STUDENT_DROPPING_OFF;
                    studentRecord.UpdatedOn = DateTime.Now;
                    oneStudentPerSectionPair.OneStudentPerSectionList.Add(studentRecord);
                    courseSection = SetSectionToInactive(courseSection);
                    oneStudentPerSectionPair.CourseSections.Add(courseSection);
                } 
                else
                    throw new Exception("Issue with sections to inactivate");

                OneStudentPerSectionPairListToUpdate.Add(oneStudentPerSectionPair);
            }

            if (OneStudentPerSectionPairListToUpdate.Any())
                OneStudentPerSectionPairListToUpdate = p.UpdateOneStudentPerSectionRecordsToInactivateCommand.ExecuteCommand(OneStudentPerSectionPairListToUpdate);
        }

        public CourseSection SetSectionToInactive(CourseSection courseSection)
        {
            courseSection.StatusID = SectionStatus.INACTIVE;
            courseSection.Active = false;
            courseSection.IsManagedByARB = false;

            return courseSection;
        }

        public List<StudentSectionError> HandleErrorInactivationRecords(List<OneStudentPerSectionPair> OneStudentPerSectionPairListWithErrors, Parameters p)
        {
            if (!OneStudentPerSectionPairListWithErrors.Any()) return new List<StudentSectionError>();

            // TODO: possible source for bug with redundant errors
            foreach (var pairRecord in OneStudentPerSectionPairListWithErrors)
                foreach(var studentRecord in pairRecord.OneStudentPerSectionList)
                    _oneStudentPerSectionErrorChecking.SaveErrorRecordToMemory(studentRecord, pairRecord.StatusID, p.Job);

            return _oneStudentPerSectionErrorChecking.SaveErrorRecordsToDbAndRemoveThemFromMemory(p.AddStudentSectionErrorCommand, p.Job);
        }

        public OneStudentPerSectionPair CreateCourseSectionsToAddToCampusVueAndSaveSectionsAndStudentRecordsToDb(Parameters p, IConfigurationRepository configurationRepository)
        {
            var oneStudentPerSectionPair = new OneStudentPerSectionPair();

            if (!p.OneStudentPerSectionRecordToCreate.Any())
                return oneStudentPerSectionPair;

            foreach (var studentSection in p.OneStudentPerSectionRecordToCreate)
            {
                Guid sectionGuid = Guid.NewGuid();
                studentSection.SectionGuid = sectionGuid;
                CourseSection courseSection = new CourseSection()
                {
                    Id = sectionGuid,
                    JobID = p.Job.Id,
                    CampusCode = GeneralStatus.CAMPUS_ONLINE,
                    CourseCode = studentSection.CourseCode,
                    AdCourseID = studentSection.AdCourseID,
                    StartDate = studentSection.StartDate,
                    ExternalSystem = GeneralStatus.ARB_JOB,
                    ExternalSystemId = sectionGuid.ToString(),
                    TermCode = studentSection.Term,
                    AdTermID = studentSection.AdTermID,
                    // This value should always be one
                    //MaximumNumberOfStudents = new Calculator().GetMaxNumberOfStudentsPerSection(firstRecord.TargetStudentCount, firstRecord.GroupNumber, firstRecord.GroupTargetStudentCount),
                    MaximumNumberOfStudents = 1,
                    PassFailSetting = GeneralStatus.PassFailType.NoOptionForPassFail,
                    PrimaryInstructor = _configInstance.GetDefaultInstructorEmail(),
                    SectionCode = studentSection.SectionCode,
                    SectionDeliveryMethod = GeneralStatus.ONLINE_DELIVERY_METHOD,
                    HasSectionBeenCreatedOrUpdatedInCampusVue = false,
                    GroupCategory = studentSection.GroupCategory,
                    StatusID = SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE,
                    Active = true,
                    IsManagedByARB = true,
                    ForecastSection = false,
                    LmsExtractStatus = studentSection.LmsExtractStatus,
                    SyLmsVendorID = studentSection.SysLmsVendorID,
                    EndDate = studentSection.EndDate,
                    FinalRegStudents = 1,
                    CreatedBy = GeneralStatus.ARB_JOB,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = GeneralStatus.ARB_JOB,
                    UpdatedOn = DateTime.Now
                };
                oneStudentPerSectionPair.CourseSections.Add(courseSection);
                oneStudentPerSectionPair.OneStudentPerSectionList.Add(studentSection);
                //p.CalcModel.CourseSections.Add(courseSection);
                //p.CalcModel.PreviewOneStudentPerSectionRecords.Add(studentSection);
            }
            return p.AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand.ExecuteCommand(oneStudentPerSectionPair);
        }
    }
}
