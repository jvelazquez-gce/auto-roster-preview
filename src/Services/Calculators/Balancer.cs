using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Services.CVue;
using Services.LastClassTogether;
using Services.OneStudentPerSection;
using Domain.Entities;
using Infrastructure.Utilities;
using Newtonsoft.Json;
using Domain.Interfaces.Infrastructure.Database.Commands;

namespace Services.Calculators
{
    public class Balancer : IBalancer
    {
        private readonly ICVueSectionsCalculator _cVueSectionsCalculator;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly IAddPreviewStudentAndUpdateCourseSectionsCommand _addPreviewStudentAndUpdateCourseSectionsCommand;
        private readonly IAddLiveModeStudentAndCourseSectionsCommand _addLiveModeStudentAndCourseSectionsCommand;
        private readonly ISectionErrorRepository _sectionErrorRepository;
        private readonly ICourseSectionCalculator _courseSectionCalculator;
        private readonly IErrorChecking _errorChecking;
        private readonly IGeneralLastClassTogetherCalculator _generalLastClassTogetherCalculator;
        private readonly IGroupCalculator _groupCalculator;
        private readonly INoGroupCalculator _noGroupCalculator;
        private readonly IOneStudentPerSectionHandler _oneStudentPerSectionHandler;
        private readonly ILastClassTogetherHandler _lastClassTogetherHandler;
        private readonly ICampusVueSetUpHandler _campusVueSetUpHandler;

        public Balancer(ICVueSectionsCalculator cVueSectionsCalculator,
            IConfigurationRepository configurationRepository,
            IAddPreviewStudentAndUpdateCourseSectionsCommand addPreviewStudentAndUpdateCourseSectionsCommand,
            IAddLiveModeStudentAndCourseSectionsCommand addLiveModeStudentAndCourseSectionsCommand,
            ISectionErrorRepository sectionErrorRepository,
            ICourseSectionCalculator courseSectionCalculator,
            IErrorChecking errorChecking,
            IGeneralLastClassTogetherCalculator generalLastClassTogetherCalculator,
            IGroupCalculator groupCalculator,
            INoGroupCalculator oGroupCalculator,
            IOneStudentPerSectionHandler oneStudentPerSectionHandler,
            ILastClassTogetherHandler lastClassTogetherHandler,
            ICampusVueSetUpHandler campusVueSetUpHandler)
        {
            _cVueSectionsCalculator = cVueSectionsCalculator;
            _configurationRepository = configurationRepository;
            _addPreviewStudentAndUpdateCourseSectionsCommand = addPreviewStudentAndUpdateCourseSectionsCommand;
            _addLiveModeStudentAndCourseSectionsCommand = addLiveModeStudentAndCourseSectionsCommand;
            _sectionErrorRepository = sectionErrorRepository;
            _courseSectionCalculator = courseSectionCalculator;
            _errorChecking = errorChecking;
            _generalLastClassTogetherCalculator = generalLastClassTogetherCalculator;
            _groupCalculator = groupCalculator;
            _noGroupCalculator = oGroupCalculator;
            _oneStudentPerSectionHandler = oneStudentPerSectionHandler;
            _lastClassTogetherHandler = lastClassTogetherHandler;
            _campusVueSetUpHandler = campusVueSetUpHandler;
        }

        public void CheckForRecordsWithDataErrorsAndRemoveThem(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            Job job, 
            int errorCheckType)
        {
            var listOfRecordsToRemove = new List<PreLoadStudentSection>();

            foreach (var record in sectionsWithSameCourseAndStartDate)
            {
                if (errorCheckType == ErrorCheckType.FIRST_CHECK)
                {
                    if (_errorChecking.IsRecordCleanFirstCheck(record, _sectionErrorRepository, _configurationRepository, job).ErrorFound)
                    {
                        listOfRecordsToRemove.Add(record);
                    }
                }
                else if (errorCheckType == ErrorCheckType.SECOND_CHECK)
                {
                    if (_errorChecking.IsRecordCleanSecondCheck(record, _sectionErrorRepository, job).ErrorFound)
                    {
                        listOfRecordsToRemove.Add(record);
                    }
                }
            }

            if (_sectionErrorRepository.NumberOfErrorsThatNeedToBeSaved() > 0) _sectionErrorRepository.Save();

            listOfRecordsToRemove.ForEach(r => sectionsWithSameCourseAndStartDate.Remove(r));
        }

        public CalcModel ExcecuteLogicForUniqueCourseAndStartDate(
            int courseId, 
            DateTime startDate, 
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            Job job, 
            Parameters p = null)
        {
            var calcModel = new CalcModel() { CourseID = courseId, StartDate = startDate, JobStatusId = (int)job.StatusId };

            calcModel = new ExcludeSectionHandler().CheckIfThereAreSectionsThatCanBeExcludedAndExcludeThem(sectionsWithSameCourseAndStartDate, calcModel, job);

            var oneStudentPerSectionResults =
                _oneStudentPerSectionHandler.ProcessRecordsAndGetCalcModel(sectionsWithSameCourseAndStartDate, _configurationRepository, calcModel, p);

            CheckForRecordsWithDataErrorsAndRemoveThem(sectionsWithSameCourseAndStartDate, job, ErrorCheckType.FIRST_CHECK);
            _cVueSectionsCalculator.UpdateAndAddCampusVueSections(sectionsWithSameCourseAndStartDate, calcModel, job);

            calcModel.PreloadRecordsWithoutStudentIDs = sectionsWithSameCourseAndStartDate.Where(r => r.SyStudentID == 0 || !r.SyStudentID.HasValue).ToList();
            calcModel.PreloadRecordsWithoutStudentIDs.ForEach(r => sectionsWithSameCourseAndStartDate.Remove(r));
            calcModel.CourseStartDateListOnlyHasClassesWithZeroStudents = sectionsWithSameCourseAndStartDate.Count <= 0;

            CheckForRecordsWithDataErrorsAndRemoveThem(sectionsWithSameCourseAndStartDate, job, ErrorCheckType.SECOND_CHECK);

            _lastClassTogetherHandler.ProcessData();

            calcModel = GetSectionsThatNeedToBeCreated(sectionsWithSameCourseAndStartDate, calcModel, job, p);

            var calcModelJson = JsonConvert.SerializeObject(calcModel);
            var courseStartDateListJsonMsg = $"GetSectionsThatNeedToBeCreated: {calcModelJson}";
            MyLogger.CreateLog(p, courseStartDateListJsonMsg, courseId, startDate, LogLevel.Info, job.Id, job.ProcessID);

            if (job.StatusId == JobStatus.RUNNING_LIVE_JOB)
            {
                calcModel = UpdateCalcModelWithOneStudentPerSectionRecords(calcModel, oneStudentPerSectionResults, job);
                _campusVueSetUpHandler.CreateLiveJobRecords(calcModel, job);
            }

            SaveToArbDb(calcModel, p, job);
            return calcModel;
        }

        public CalcModel UpdateCalcModelWithOneStudentPerSectionRecords(
            CalcModel calcModel, 
            OneStudentPerSectionResults oneStudentPerSectionResults, 
            Job job)
        {
            if (oneStudentPerSectionResults == null) return calcModel;

            if(oneStudentPerSectionResults.CalcModel.CourseSectionsToBeCancelled.Any())
                calcModel.CourseSectionsToBeCancelled.AddRange(oneStudentPerSectionResults.CalcModel.CourseSectionsToBeCancelled);

            if(oneStudentPerSectionResults.CalcModel.CourseSections.Any())
                calcModel.CourseSections.AddRange(oneStudentPerSectionResults.CalcModel.CourseSections);

            calcModel.OneStudentPerSectionStudentRecords = oneStudentPerSectionResults.CalcModel.OneStudentPerSectionStudentRecords;

            return calcModel;
        }

        public CalcModel GetSectionsThatNeedToBeCreated(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, 
            CalcModel calcModel, 
            Job job, 
            Parameters p)
        {
            var firstRecord = sectionsWithSameCourseAndStartDate.FirstOrDefault();
            if (firstRecord == null)
            {
                // This line of code ensures that CVue courses with zero students and instructors don't get marked as Inactive when there is not other records to process.
                var queryInputParams = new QueryInputParams()
                {
                    AdCourseID = calcModel.CourseID,
                    StartDate = calcModel.StartDate,
                };
                calcModel.CourseSectionsThatWereNotReUsed = p.GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery.ExecuteQuery(queryInputParams).ToList();
                _courseSectionCalculator.CreateForecastedSections(calcModel, job, _configurationRepository);
                return calcModel;
            }

            var exclusiveCohortCalcModel = CreateExclusiveSections(sectionsWithSameCourseAndStartDate, job);
            var nonExclusiveCohortsCalcModel = CreateNonExclusiveCohortSections(sectionsWithSameCourseAndStartDate, job);
            FillNonExclusiveCohortSectionsWithNoGroupStudentsAndCreateNoGroupSectionsIfNeeded(sectionsWithSameCourseAndStartDate, nonExclusiveCohortsCalcModel, job);
            CombineNonExclusiveCohortSectionsIfPossibleToCreateTheLeastAmountOfSections(nonExclusiveCohortsCalcModel, job);
            _generalLastClassTogetherCalculator.CombineStudentsThatTookLastClassTogetherForAllHomogeneousSections(nonExclusiveCohortsCalcModel, job);

            nonExclusiveCohortsCalcModel.PreviewStudentRecords.ForEach(r => exclusiveCohortCalcModel.PreviewStudentRecords.Add(r));
            exclusiveCohortCalcModel.TotalStudentsRegistered = nonExclusiveCohortsCalcModel.TotalStudentsRegistered;
            exclusiveCohortCalcModel.MaxStudentsPerSection = nonExclusiveCohortsCalcModel.MaxStudentsPerSection;
            exclusiveCohortCalcModel.TotalSectionsNeeded = nonExclusiveCohortsCalcModel.TotalSectionsNeeded;

            _courseSectionCalculator.CreateCourseSectionsInPrepForCVueCreation(
                exclusiveCohortCalcModel, 
                calcModel.CourseID, 
                calcModel.StartDate, 
                job, 
                _sectionErrorRepository, 
                _configurationRepository);

            exclusiveCohortCalcModel.CourseSectionsToBeCancelled = calcModel.CourseSectionsToBeCancelled;
            exclusiveCohortCalcModel.SectionsToExclude = calcModel.SectionsToExclude;
            exclusiveCohortCalcModel.CourseID = calcModel.CourseID;
            exclusiveCohortCalcModel.StartDate = calcModel.StartDate;
            exclusiveCohortCalcModel.JobStatusId = calcModel.JobStatusId;

            if (calcModel.CourseSections == null || 
                !calcModel.CourseSections.Any() || 
                calcModel.LastClassGroupStudentSections == null || 
                !calcModel.LastClassGroupStudentSections.Any())
            {
                return exclusiveCohortCalcModel;
            }

            // Add Last class group together CourseSections and LastClassGroupStudentSections
            exclusiveCohortCalcModel.CourseSections.AddRange(calcModel.CourseSections);
            exclusiveCohortCalcModel.LastClassGroupStudentSections.AddRange(calcModel.LastClassGroupStudentSections);

            return exclusiveCohortCalcModel;
        }

        public CalcModel CreateNonExclusiveCohortSections(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, Job job)
        {
            var calcModel = new CalcModel();
            foreach(var cohort in Group.NON_EXCLUSIVE_COHORT_KEY_LIST)
            {
                var groupNameCounterSuffix = cohort == Group.FRIEND_COHORT 
                    ? GeneralStatus.BASE_COUNTER 
                    : GeneralStatus.INCLUSIVE_AND_EXCLUSIVE_BASE_COUNTER;

                var cohortCalcModel = _groupCalculator.GetCalculatedSectionsModel(sectionsWithSameCourseAndStartDate, cohort, groupNameCounterSuffix, job, _configurationRepository);
                cohortCalcModel.PreviewStudentRecords.ForEach(r => calcModel.PreviewStudentRecords.Add(r));
            }
            return calcModel;
        }

        public void CombineNonExclusiveCohortSectionsIfPossibleToCreateTheLeastAmountOfSections(CalcModel nonExclusiveCohortsCalcModel, Job job)
        {
            var nonExclusiveCohortPreviewRecords = _groupCalculator.NonExclusiveCohortPreviewStudentRecords(nonExclusiveCohortsCalcModel);
            _groupCalculator.CombineSmallestToBiggestGroupSectionsIfPossibleToCreateTheLeastAmountOfSections(nonExclusiveCohortsCalcModel, nonExclusiveCohortPreviewRecords, job);
        }
        
        public CalcModel CreateExclusiveSections(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, Job job)
        {
            return _groupCalculator.GetCalculatedSectionsModel(sectionsWithSameCourseAndStartDate, Group.EXCLUSIVE_COHORT, GeneralStatus.INCLUSIVE_AND_EXCLUSIVE_BASE_COUNTER, job, _configurationRepository);
        }

        public void FillNonExclusiveCohortSectionsWithNoGroupStudentsAndCreateNoGroupSectionsIfNeeded(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            CalcModel calculatedModel, Job job)
        {
            _noGroupCalculator.GetCalculatedSectionsModel(sectionsWithSameCourseAndStartDate, calculatedModel, job);
        }

        public void SaveToArbDb(CalcModel calculatedModel, Parameters p, Job job)
        {
            if (calculatedModel.JobStatusId == JobStatus.PREVIEW_JOB_RUNNING)
            {
                _addPreviewStudentAndUpdateCourseSectionsCommand.Execute(calculatedModel, job);
            }
            else if (calculatedModel.JobStatusId == JobStatus.RUNNING_LIVE_JOB)
            {
                _addLiveModeStudentAndCourseSectionsCommand.Execute(calculatedModel, job);
            }
        }
    }
}
