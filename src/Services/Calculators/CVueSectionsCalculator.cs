using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.Utilities;
using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces.Infrastructure.Database.Commands;

namespace Services.Calculators
{
    public class CVueSectionsCalculator : ICVueSectionsCalculator
    {
        private readonly Parameters _p;
        private readonly IDeleteOrInactivateCVueSectionsTxCommand _deleteOrInactivateCVueSectionsTxCommand;
        private readonly IAddCourseSectionsTxCommand _addCourseSectionsTxCommand;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ICalculator _calculator;
        private readonly IConfigInstance _configInstance;

        public CVueSectionsCalculator(
            Parameters p,
            IDeleteOrInactivateCVueSectionsTxCommand deleteOrInactivateCVueSectionsTxCommand,
            IAddCourseSectionsTxCommand addCourseSectionsTxCommand,
            IConfigurationRepository configurationRepository,
            ICalculator calculator,
            IConfigInstance configInstance)
        {
            _p = p;
            _deleteOrInactivateCVueSectionsTxCommand = deleteOrInactivateCVueSectionsTxCommand;
            _addCourseSectionsTxCommand = addCourseSectionsTxCommand;
            _configurationRepository = configurationRepository;
            _calculator = calculator;
            _configInstance = configInstance;
        }

        public void UpdateAndAddCampusVueSections(List<PreLoadStudentSection> initialStudentRawData, 
            CalcModel calcModel,
            Job job)
        {
            using var context = new ARBDb(_p.AppSettings.DbConnectionString);
            var executionStrategy = context.Database.CreateExecutionStrategy();
            executionStrategy.Execute(
                () =>
                {
                    RunUpdateAndAddCampusVueSections(context, initialStudentRawData, calcModel, job);
                });
        }

        private void RunUpdateAndAddCampusVueSections(ARBDb context, List<PreLoadStudentSection> initialStudentRawData, 
            CalcModel calcModel, Job job)
        {
            using var dbContextTransaction = context.Database.BeginTransaction();
            try
            {
                AddUpdateAndInactivateNonSuperSectionsWithInstructorsToReUse(initialStudentRawData, calcModel, job);
                AddUpdateAndInactivateSectionsWithoutInstructorsToCancel(initialStudentRawData, calcModel, job);

                dbContextTransaction.Commit();
            }
            catch (Exception)
            {
                dbContextTransaction.Rollback();
                throw;
            }

        }

        private List<int> AddUpdateAndInactivateNonSuperSectionsWithInstructorsToReUse(List<PreLoadStudentSection> initialStudentRawData, 
            CalcModel calcModel,
            Job job)
        {
            var NonSuperSectionsAdClassSchedIDsWithInstructorsToBeDeletedAndReAdded = new List<int>();
            NonSuperSectionsAdClassSchedIDsWithInstructorsToBeDeletedAndReAdded = 
                initialStudentRawData
                .Where(r => r.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER)
                .Where(r => r.HasInstructor)
                .Select(r => r.AdClassSchedID)
                .Distinct()
                .ToList();

            var queryInputParams = new QueryInputParams()
            {
                ExcludeSuperSection = true,
                AdCourseID = calcModel.CourseID,
                StartDate = calcModel.StartDate,
                IdsToDelete = NonSuperSectionsAdClassSchedIDsWithInstructorsToBeDeletedAndReAdded,
                Job = job,
            };
            // After this method runs, all CVue non-super sections with the course/start-date combo are either deleted or inactive in the CourseSections table.
            List<CourseSection> deletedSections = _deleteOrInactivateCVueSectionsTxCommand.Execute(queryInputParams);

            if (NonSuperSectionsAdClassSchedIDsWithInstructorsToBeDeletedAndReAdded.Count == 0) return new List<int>();

            var courseSections = new List<CourseSection>();
            foreach (var adClassSchedId in NonSuperSectionsAdClassSchedIDsWithInstructorsToBeDeletedAndReAdded)
            {
                PreLoadStudentSection firstRecord = initialStudentRawData.FirstOrDefault(r => r.AdClassSchedID == adClassSchedId);
                Guid guid = GetSectionID(deletedSections, adClassSchedId);
                var courseSection = new CourseSection()
                {
                    Id = guid,
                    JobID = job.Id,
                    CampusCode = GeneralStatus.CAMPUS_ONLINE,
                    CourseCode = firstRecord.CourseCode,
                    AdCourseID = firstRecord.AdCourseID,
                    StartDate = firstRecord.StartDate,
                    ExternalSystem = GeneralStatus.ARB_JOB,
                    ExternalSystemId = guid.ToString(),
                    TermCode = firstRecord.Term,
                    MaximumNumberOfStudents = _calculator.GetMaxNumberOfStudentsPerSection(firstRecord.TargetStudentCount, firstRecord.GroupNumber, firstRecord.GroupTargetStudentCount),
                    PassFailSetting = GeneralStatus.PassFailType.NoOptionForPassFail,
                    PrimaryInstructor = firstRecord.PrimaryInstructor,
                    InstructorType = firstRecord.InstructorType,
                    SectionCode = firstRecord.SectionCode,
                    SectionDeliveryMethod = GeneralStatus.ONLINE_DELIVERY_METHOD,
                    HasInstructor = firstRecord.HasInstructor,
                    HasSectionBeenCreatedOrUpdatedInCampusVue = false,
                    GroupCategory = GetGroupCategory(adClassSchedId, initialStudentRawData),
                    StatusID = SectionStatus.READY_TO_BE_REUSED,
                    Active = true,
                    LmsExtractStatus = firstRecord.LmsExtractStatus,
                    SyLmsVendorID = firstRecord.SysLmsVendorID,
                    EndDate = firstRecord.EndDate,
                    AdClassSchedId = firstRecord.AdClassSchedID,
                    OriginalCVueMaximumNumberOfStudents = firstRecord.CVueMaxStudents,
                    AdTeacherID = firstRecord.AdTeacherID,
                    InstructorAssignedStatusID = firstRecord.InstructorAssignedStatusID,
                    CreatedBy = GeneralStatus.ARB_JOB,
                    CreatedOn = DateTime.Now,
                    UpdatedBy = GeneralStatus.ARB_JOB,
                    UpdatedOn = DateTime.Now
                };
                courseSections.Add(courseSection);
            }
            _addCourseSectionsTxCommand.ExecuteCommand(courseSections);
            return NonSuperSectionsAdClassSchedIDsWithInstructorsToBeDeletedAndReAdded;
        }

        private Guid GetSectionID(List<CourseSection> deletedCoursesThatNeedToBeReAdded, int adClassSchedID)
        {
            if (deletedCoursesThatNeedToBeReAdded == null || deletedCoursesThatNeedToBeReAdded.Count == 0) return Guid.NewGuid();

            List<CourseSection> deletedSectionsWithUniqueAdClassSchedID = deletedCoursesThatNeedToBeReAdded
                .Where(s => s.AdClassSchedId == adClassSchedID)
                .ToList();

            return deletedSectionsWithUniqueAdClassSchedID.Count > 0 
                ? deletedSectionsWithUniqueAdClassSchedID.OrderByDescending(s => s.JobID).First().Id 
                : Guid.NewGuid();
        }

        private int GetGroupCategory(int adClassSchedID, 
            List<PreLoadStudentSection> initialStudentRawData)
        {
            var firstRecord = new PreLoadStudentSection();
            List<PreLoadStudentSection> groupTypeList = initialStudentRawData
                .Where(r => r.AdClassSchedID == adClassSchedID)
                .Where(r => r.GroupTypeKey.HasValue )
                .Where(r => r.GroupTypeKey != 0)
                .ToList();
            int numberOfCohortRecords = groupTypeList.Count();

            if (numberOfCohortRecords >= _configInstance.GetCohortMinSize())
            {
                List<Guid> cohortNumbers = groupTypeList.Select(r => (Guid)r.GroupNumber).Distinct().ToList();
                foreach (var num in cohortNumbers)
                {
                    int numberOfRecordsWithTheSameCohort = groupTypeList.Where(r => (Guid)r.GroupNumber == num).Count();
                    if (numberOfRecordsWithTheSameCohort >= _configInstance.GetCohortMinSize())
                    {
                        PreLoadStudentSection record = groupTypeList.Where(r => r.GroupNumber == num).First();
                        if (record.GroupTypeKey == Group.EXCLUSIVE_COHORT) return ARBGroupCategory.EXCLUSIVE;
                        else if (record.GroupTypeKey == Group.INCLUSIVE_COHORT) return ARBGroupCategory.INCLUSIVE;
                        else if (record.GroupTypeKey == Group.FRIEND_COHORT) return ARBGroupCategory.FRIEND;
                    }
                }
            }
            return ARBGroupCategory.GENERAL;
        }

        private void AddUpdateAndInactivateSectionsWithoutInstructorsToCancel(
            List<PreLoadStudentSection> initialStudentRawData, 
            CalcModel calcModel, Job job)
        {
            var SectionsAdClassSchedIDsWithoutInstructorsToBeDeletedAndReAdded = new List<int>();

            SectionsAdClassSchedIDsWithoutInstructorsToBeDeletedAndReAdded = 
                initialStudentRawData
                .Where(r => !r.HasInstructor || (r.HasInstructor && r.SectionCode == GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER))
                .Select(r => r.AdClassSchedID)
                .Distinct()
                .ToList();

            if (SectionsAdClassSchedIDsWithoutInstructorsToBeDeletedAndReAdded.Count == 0) return; 

            var queryInputParams = new QueryInputParams()
            {
                ExcludeSuperSection = false,
                AdCourseID = calcModel.CourseID,
                StartDate = calcModel.StartDate,
                IdsToDelete = SectionsAdClassSchedIDsWithoutInstructorsToBeDeletedAndReAdded,
                Job = job,
            };
            // After this method runs, all CVue non-super sections with the course/start-date combo are either deleted or inactive in the CourseSections table.
            List<CourseSection> deletedSections = _deleteOrInactivateCVueSectionsTxCommand.Execute(queryInputParams);

            foreach (var adClassSchedId in SectionsAdClassSchedIDsWithoutInstructorsToBeDeletedAndReAdded)
            {
                PreLoadStudentSection firstRecord = initialStudentRawData.FirstOrDefault(r => r.AdClassSchedID == adClassSchedId);
                Guid guid = GetSectionID(deletedSections, adClassSchedId);
                CourseSection sectionToCancel = new CourseSection()
                {
                    Id = guid,
                    JobID = job.Id,
                    CampusCode = GeneralStatus.CAMPUS_ONLINE,
                    CourseCode = firstRecord.CourseCode,
                    AdCourseID = calcModel.CourseID,
                    AdTermID = firstRecord.AdTermID,
                    StartDate = calcModel.StartDate,
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
                    GroupCategory = GetGroupCategory(adClassSchedId, initialStudentRawData),
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
                calcModel.CourseSectionsToBeCancelled.Add(sectionToCancel);
            }
        }
    }
}
