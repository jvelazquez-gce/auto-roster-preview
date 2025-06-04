using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.Utilities;

namespace Services.Calculators
{
    public class CourseSectionCalculator : ICourseSectionCalculator
    {
        private readonly Parameters _p;
        private readonly IConfigInstance _configInstance;
        private readonly ICalculator _calculator;
        private readonly IErrorChecking _errorChecking;
        private readonly IForecastSectionCalculator _forecastSectionCalculator;

        public CourseSectionCalculator(
            Parameters p, 
            IConfigInstance configInstance, 
            ICalculator calculator,
            IErrorChecking errorChecking,
            IForecastSectionCalculator forecastSectionCalculator)
        {
            _p = p;
            _configInstance = configInstance;
            _calculator = calculator;
            _errorChecking = errorChecking;
            _forecastSectionCalculator = forecastSectionCalculator;
        }

        public void CreateCourseSectionsInPrepForCVueCreation(CalcModel calculatedModel, int courseId, DateTime startDate,
            Job job, ISectionErrorRepository sectionErrorRepository, IConfigurationRepository configurationRepository)
        {
            CreateCourseSections(calculatedModel, job, configurationRepository);
            ReUsePreviouslyCreatedSections(calculatedModel, courseId, startDate, job, configurationRepository);
            UpdateStudentSectionsWithCourseSectionGuid(calculatedModel, job, sectionErrorRepository);
            CreateForecastedSections(calculatedModel, job, configurationRepository);
        }

        public void CreateCourseSections(CalcModel calculatedModel, Job job, IConfigurationRepository configurationRepository)
        {
            var distinctSectionCode = calculatedModel.PreviewStudentRecords
                .Select(s => s.SectionCode)
                .Distinct()
                .ToList();

            if (distinctSectionCode.Count == 0) return;

            foreach (var sectionCode in distinctSectionCode)
            {
                var firstRecord = calculatedModel.PreviewStudentRecords
                    .First(s => s.SectionCode == sectionCode);

                AddCourseSection(sectionCode, firstRecord, calculatedModel, job, configurationRepository, false);
            }
        }

        public void CreateForecastedSections(CalcModel calculatedModel, Job job, IConfigurationRepository configurationRepository)
        {
            if (job.StatusId != JobStatus.PREVIEW_JOB_RUNNING) return;

            var firstRecord = _forecastSectionCalculator.GetForecastRecord(calculatedModel);

            if ( firstRecord == null ) return;
            calculatedModel.ForecastSectionsNeeded = _forecastSectionCalculator.GetForeCastSectionsNeeded(firstRecord, calculatedModel, job);
            if (calculatedModel.ForecastSectionsNeeded <= 0) return;

            if (calculatedModel.ForecastSectionsNeeded <= _configInstance.MaxNumberOfForecastSections())
            {
                var counter = _forecastSectionCalculator.GetNextSectionNumberToCreate(calculatedModel);
                var sectionNames = new SectionCodeCalculator().CreateGroupNames(calculatedModel.ForecastSectionsNeeded, GeneralStatus.NOCOHORT_NOLOCATION_NAME_PREFIX, ref counter);
                firstRecord.GroupCategory = ARBGroupCategory.GENERAL;
                sectionNames.ForEach(s => AddCourseSection(s, firstRecord, calculatedModel, job, configurationRepository, true));
            }
            else
            {
                var message =
                    $"ARB calculated that {calculatedModel.ForecastSectionsNeeded} " +
                    $"forecast sections need to be created. However, the max allowed is " + 
                    $"{_configInstance.MaxNumberOfForecastSections()} " +
                    $". CourseID: {calculatedModel.CourseID}, Start date: {calculatedModel.StartDate}";
            }
        }

        private void AddCourseSection(string sectionCode, PreviewStudentSection firstRecord, CalcModel calculatedModel, Job job, IConfigurationRepository configurationRepository, bool forecastSection)
        {
            var sectionGuid = Guid.NewGuid();
            var courseSection = new CourseSection()
            {
                Id = sectionGuid,
                JobID = job.Id,
                CampusCode = GeneralStatus.CAMPUS_ONLINE,
                CourseCode = firstRecord.CourseCode,
                AdCourseID = firstRecord.AdCourseID,
                StartDate = firstRecord.StartDate,
                ExternalSystem = GeneralStatus.ARB_JOB,
                ExternalSystemId = sectionGuid.ToString(),
                TermCode = firstRecord.Term,
                AdTermID = firstRecord.AdTermID,
                MaximumNumberOfStudents = _calculator.GetMaxNumberOfStudentsPerSection(firstRecord.TargetStudentCount, firstRecord.GroupNumber, firstRecord.GroupTargetStudentCount),
                PassFailSetting = GeneralStatus.PassFailType.NoOptionForPassFail,
                PrimaryInstructor = _configInstance.GetDefaultInstructorEmail(),
                SectionCode = sectionCode,
                SectionDeliveryMethod = GeneralStatus.ONLINE_DELIVERY_METHOD,
                HasSectionBeenCreatedOrUpdatedInCampusVue = false,
                GroupCategory = firstRecord.GroupCategory,
                StatusID = SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE,
                Active = true,
                ForecastSection = forecastSection,
                LmsExtractStatus = firstRecord.LmsExtractStatus,
                SyLmsVendorID = firstRecord.SysLmsVendorID,
                EndDate = firstRecord.EndDate,
                CreatedBy = GeneralStatus.ARB_JOB,
                CreatedOn = DateTime.Now,
                UpdatedBy = GeneralStatus.ARB_JOB,
                UpdatedOn = DateTime.Now
            };
            calculatedModel.CourseSections.Add(courseSection);
        }

        public void ReUsePreviouslyCreatedSections(CalcModel calcModel, int courseId, DateTime startDate, Job job, IConfigurationRepository configurationRepository)
        {
            var queryInputParams = new QueryInputParams()
            {
                AdCourseID = courseId,
                StartDate = startDate,
            };
            var courseSectionToReUsedList = _p.GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery.ExecuteQuery(queryInputParams).ToList();
            courseSectionToReUsedList = ReSortCoursesToBeReUsed(courseSectionToReUsedList, _configInstance.GetDefaultInstructorID());
            if (courseSectionToReUsedList == null || courseSectionToReUsedList.Count == 0) return;

            var updatedCourseSectionList = new List<CourseSection>();
            courseSectionToReUsedList.ForEach(r => ReUseSectionsAndIdentifyNotReUsedSectionsList(calcModel, r, job, updatedCourseSectionList));
            updatedCourseSectionList.ForEach(s => calcModel.CourseSections.Add(s));
        }

        public List<CourseSection> ReSortCoursesToBeReUsed(List<CourseSection> courseSectionList, int defaultTecherID)
        {
            var prioritizedCoursesList = new List<CourseSection>();

            var coursesWithFTTeachers = courseSectionList
                .Where(c => c.HasInstructor)
                .Where(c => c.AdTeacherID != defaultTecherID)
                .Where(c => c.InstructorType == InstructorType.FULL_TIME)
                .ToList();

            var coursesWithAdjunctTeachers = courseSectionList
                .Where(c => c.HasInstructor)
                .Where(c => c.AdTeacherID != defaultTecherID)
                .Where(c => c.InstructorType == InstructorType.ADJUNCT)
                .ToList();

            var coursesWithRealTeachersWithNoInstructorType = courseSectionList
                .Where(c => c.HasInstructor)
                .Where(c => c.AdTeacherID != defaultTecherID)
                .Where(c => !c.InstructorType.HasValue || c.InstructorType == 0)
                .ToList();

            var coursesWithDefaultTeachers = courseSectionList
                .Where(c => c.HasInstructor)
                .Where(c => c.AdTeacherID == defaultTecherID)
                .ToList();

            coursesWithFTTeachers.ForEach(c => courseSectionList.Remove(c));
            coursesWithAdjunctTeachers.ForEach(c => courseSectionList.Remove(c));
            coursesWithRealTeachersWithNoInstructorType.ForEach(c => courseSectionList.Remove(c));
            coursesWithDefaultTeachers.ForEach(c => courseSectionList.Remove(c));
            
            coursesWithFTTeachers.ForEach(c => prioritizedCoursesList.Add(c));
            coursesWithAdjunctTeachers.ForEach(c => prioritizedCoursesList.Add(c));
            coursesWithRealTeachersWithNoInstructorType.ForEach(c => prioritizedCoursesList.Add(c));
            coursesWithDefaultTeachers.ForEach(c => prioritizedCoursesList.Add(c));
            courseSectionList.ForEach(c => prioritizedCoursesList.Add(c));

            return prioritizedCoursesList;
        }

        public void ReUseSectionsAndIdentifyNotReUsedSectionsList(CalcModel calcModel, CourseSection sectionToBeReUsed, Job job, List<CourseSection> updatedCourseSectionLits)
        {
            var firstCourseSection = calcModel.CourseSections
                .Where(r => r.SectionCode == sectionToBeReUsed.SectionCode)
                .FirstOrDefault(r => r.GroupCategory == sectionToBeReUsed.GroupCategory);

            if (firstCourseSection != null)
            {
                calcModel.CourseSections.Remove(firstCourseSection);
                UpdateCourseSection(calcModel, firstCourseSection, sectionToBeReUsed, job);
                updatedCourseSectionLits.Add(firstCourseSection);
                return;
            }

            //TODO: need to ensure that all records have the NewGroupCategory set
            firstCourseSection = calcModel.CourseSections.FirstOrDefault(r => r.GroupCategory == sectionToBeReUsed.GroupCategory);
            if (firstCourseSection != null)
            {
                calcModel.CourseSections.Remove(firstCourseSection);
                UpdateCourseSection(calcModel, firstCourseSection, sectionToBeReUsed, job);
                updatedCourseSectionLits.Add(firstCourseSection);
                return;
            }

            calcModel.CourseSectionsThatWereNotReUsed.Add(sectionToBeReUsed);
        }

        public void UpdateCourseSection(CalcModel calcModel, CourseSection firstCourseSection, CourseSection sectionToBeReUsed, Job job)
        {
            firstCourseSection.Id = sectionToBeReUsed.Id;
            firstCourseSection.AdClassSchedId = sectionToBeReUsed.AdClassSchedId;
            firstCourseSection.PrimaryInstructor = sectionToBeReUsed.PrimaryInstructor;
            firstCourseSection.HasInstructor = sectionToBeReUsed.HasInstructor;
            firstCourseSection.InstructorType = sectionToBeReUsed.InstructorType;
            firstCourseSection.OriginalCVueMaximumNumberOfStudents = sectionToBeReUsed.OriginalCVueMaximumNumberOfStudents;
            firstCourseSection.AdTeacherID = sectionToBeReUsed.AdTeacherID;

            if (sectionToBeReUsed.InstructorAssignedStatusID.HasValue) 
                firstCourseSection.InstructorAssignedStatusID = (int)sectionToBeReUsed.InstructorAssignedStatusID;

            firstCourseSection.LmsExtractStatus = sectionToBeReUsed.LmsExtractStatus;
            firstCourseSection.SyLmsVendorID = sectionToBeReUsed.SyLmsVendorID;

            if (firstCourseSection.AdClassSchedId.HasValue && firstCourseSection.AdClassSchedId != 0)
            {
                firstCourseSection.StatusID = firstCourseSection.MaximumNumberOfStudents != firstCourseSection.OriginalCVueMaximumNumberOfStudents 
                    ? SectionStatus.READY_TO_UPDATE_SECTION_MAX_STUDENTS_IN_CVUE 
                    : SectionStatus.SECTION_DOES_NOT_NEED_TO_BE_UPDATED_BECAUSE_BOTH_ARB_AND_CVUE_MAX_NUMBERS_ARE_EQUAL;
            }
            else
                firstCourseSection.StatusID = SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE;

            var previewRecordsList = calcModel.PreviewStudentRecords
                .Where(r => r.SectionCode == firstCourseSection.SectionCode)
                .ToList();

            foreach (var record in previewRecordsList)
            {
                calcModel.PreviewStudentRecords.Remove(record);
                record.SectionGuid = sectionToBeReUsed.Id;
                record.SectionCode = sectionToBeReUsed.SectionCode;
                record.AdTeacherID = sectionToBeReUsed.AdTeacherID;
                record.NewSectionId = sectionToBeReUsed.AdClassSchedId;

                record.StatusID = record.OldSectionId == record.NewSectionId 
                    ? StudentSectionStatus.DOES_NOT_NEED_TO_BE_TRANSFER_BECAUSE_SOURCE_AND_DESTINATION_ARE_EQUAL 
                    : StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE;

                calcModel.PreviewStudentRecords.Add(record);
            }

            firstCourseSection.SectionCode = sectionToBeReUsed.SectionCode;
            firstCourseSection.CreatedOn = sectionToBeReUsed.CreatedOn;
            firstCourseSection.FinalRegStudents = previewRecordsList.Count;
        }

        public void UpdateStudentSectionsWithCourseSectionGuid(CalcModel calculatedModel, Job job, ISectionErrorRepository sectionErrorRepository)
        {
            var distinctSectionCode = calculatedModel.PreviewStudentRecords
                .Select(s => s.SectionCode)
                .Distinct()
                .ToList();

            if (distinctSectionCode.Count == 0) return;

            foreach (var sectionCode in distinctSectionCode)
            {
                var listOfStudentsWithUniqueSection = calculatedModel.PreviewStudentRecords
                    .Where(r => r.SectionCode == sectionCode)
                    .ToList();

                var courseSectionList = calculatedModel.CourseSections
                    .Where(s => s.SectionCode == sectionCode)
                    .ToList();

                if (courseSectionList.Count == 0 || courseSectionList.Count > 1)
                #region error found details
                {
                    var sectionGuids = string.Empty;
                    if (courseSectionList.Count > 1)
                        sectionGuids = string.Join(",", courseSectionList.Select(r => r.Id.ToString()).ToArray());
                    var param = new object[6] { courseSectionList.Count, sectionCode, job.Id, System.Reflection.MethodBase.GetCurrentMethod().Name, job.StatusId, sectionGuids };
                    var msg = string.Format("There were {0} instances of section code: {1} in the calculatedModel.CourseSections list. " +
                                            "Job id: {2}, Method name: {3}, Job mode: {4}. Here is the ids for the course sections: {5}", param);
                    //log.Error(msg);
                    if (courseSectionList.Count > 1)
                    {
                        foreach (var course in courseSectionList)
                        {
                            calculatedModel.CourseSections.Remove(course);
                            course.StatusID = SectionStatus.ERROR_DUPLICATE_SECTION_CODE_NAMES;
                            course.Active = false;
                            calculatedModel.CourseSections.Add(course);
                        }
                    }
                    foreach (var record in listOfStudentsWithUniqueSection)
                    {
                        calculatedModel.PreviewStudentRecords.Remove(record);

                        record.StatusID = courseSectionList.Count > 1 
                            ? SectionStatus.ERROR_DUPLICATE_SECTION_CODE_NAMES 
                            : StudentSectionStatus.MISSING_COURSE_SECTION;

                        _errorChecking.SaveErrorRecord(record, record.StatusID, sectionErrorRepository);
                    }
                    if (sectionErrorRepository.NumberOfErrorsThatNeedToBeSaved() > 0) sectionErrorRepository.Save();
                    continue;
                }
                #endregion
                var courseSection = courseSectionList.First();
                calculatedModel.CourseSections.Remove(courseSection);
                foreach (var record in listOfStudentsWithUniqueSection)
                {
                    calculatedModel.PreviewStudentRecords.Remove(record);

                    if (!courseSection.HasInstructor)
                        record.AdTeacherID = null;

                    record.SectionGuid = courseSection.Id;

                    record.StatusID = record.OldSectionId == record.NewSectionId 
                        ? StudentSectionStatus.DOES_NOT_NEED_TO_BE_TRANSFER_BECAUSE_SOURCE_AND_DESTINATION_ARE_EQUAL 
                        : StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE;

                    calculatedModel.PreviewStudentRecords.Add(record);
                }
                courseSection.FinalRegStudents = listOfStudentsWithUniqueSection.Count;
                calculatedModel.CourseSections.Add(courseSection);
            }
        }
    }
}
