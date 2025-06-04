using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;

namespace Services.LastClassTogether
{
    public class LastClassTogetherCalculator : ILastClassTogetherCalculator
    {
        private List<PreLoadStudentSection> _groupList;
        private Job _job;
        private IConfigurationRepository _configurationRepository;
        private Parameters _p;
        private CalcModel _calcModel;
        private IFillSectionHandler _fillSectionHandler;
        private ICalculator _calculator;
        private ICourseSectionHandler _courseSectionHandler;
        private readonly IMapRecordsAndAddToCalcModel _mapRecordsAndAddToCalcModel;
        private readonly IReUseSectionHandler _reUseSectionHandler;

        public LastClassTogetherCalculator(List<PreLoadStudentSection> groupList,
            Job job, 
            IConfigurationRepository 
            configurationRepository,
            Parameters p,
            CalcModel calcModel,
            IFillSectionHandler fillSectionHandler,
            ICalculator calculator,
            ICourseSectionHandler courseSectionHandler,
            IMapRecordsAndAddToCalcModel mapRecordsAndAddToCalcModel,
            IReUseSectionHandler reUseSectionHandler)
        {
            _groupList = groupList;
            _job = job;
            _configurationRepository = configurationRepository;
            _p = p;
            _calcModel = calcModel;
            _fillSectionHandler = fillSectionHandler;
            _calculator = calculator;
            _courseSectionHandler = courseSectionHandler;
            _mapRecordsAndAddToCalcModel = mapRecordsAndAddToCalcModel;
            _reUseSectionHandler = reUseSectionHandler;
        }

        public void ProcessRecords(CalcModel calculatedModel)
        {
            _reUseSectionHandler.FindSectionsToReUseIfPossibleAndAssignStudents();
            CombineNewCohortStudentsIntoSections(calculatedModel);
            _fillSectionHandler.FitNonCohortsLeftIntoEmptySeatsInDb();
            _courseSectionHandler.CreateCourseSectionsAndSaveThemAndLastClassGroupStudentSectionsToDb(calculatedModel, _p);
            SetUpLiveJobDataIfLiveJobIsRunning();
        }

        private void SetUpLiveJobDataIfLiveJobIsRunning()
        {
            if (_p.Job.StatusId != JobStatus.RUNNING_LIVE_JOB) return;

            var queryInputParams = new QueryInputParams() { AdCourseID = _calcModel.CourseID, StartDate = _calcModel.StartDate };
            var courseSections = _p.GetActiveLastClassGroupCourseSectionsToCreateQuery.ExecuteQuery(queryInputParams);
            var lastClassGroupStudentSections = _p.GetLastClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);

            if (courseSections == null || !courseSections.Any() || lastClassGroupStudentSections == null || !lastClassGroupStudentSections.Any()) return;

            _calcModel.CourseSections.AddRange(courseSections);
            _calcModel.LastClassGroupStudentSections.AddRange(lastClassGroupStudentSections);
        }

        private void CombineNewCohortStudentsIntoSections(CalcModel calculatedModel)
        {
            var groupNameCounterSuffix =
                SectionCodeCalculator.GetNextLastClassTogetherSectionNumberToCreate(_p,_calcModel.CourseID, _calcModel.StartDate);

            if (!_groupList.Any()) return;

            var distinctGroupIds = _groupList.Select(r => r.GroupNumber)
                .Distinct()
                .ToList();

            foreach (var groupNumber in distinctGroupIds)
            {
                var tempGroupList = _groupList.Where(l => l.GroupNumber == groupNumber)
                    .ToList();

                if (tempGroupList.Any()) CombineGroup(tempGroupList, calculatedModel, ref groupNameCounterSuffix);
            }
        }

        private void CombineGroup(List<PreLoadStudentSection> tempGroupList, CalcModel calculatedModel, ref int groupNameCounterSuffix)
        {
            var firstRecord = tempGroupList.FirstOrDefault();
            _calculator.CalculateSectionsNeeded(firstRecord, tempGroupList, calculatedModel);
            _fillSectionHandler.FillInMemoryEmptySeats(tempGroupList, calculatedModel);

            firstRecord.GroupCategory = ARBGroupCategory.LAST_CLASS_TOGETHER_COHORT;
            if (calculatedModel.TotalSectionsNeeded > 1)
            {
                var groupNameSuffix = GeneralStatus.LAST_CLASS_TOGETHER_COHORT_PREFIX;
                _mapRecordsAndAddToCalcModel.BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(calculatedModel, 
                    firstRecord, tempGroupList, groupNameSuffix, ref groupNameCounterSuffix);
            }
            else if (calculatedModel.TotalSectionsNeeded == 1)
            {
                var sectionName = GeneralStatus.LAST_CLASS_TOGETHER_COHORT_PREFIX + groupNameCounterSuffix;
                _mapRecordsAndAddToCalcModel.AddAllStudentsToModelInOneSection(calculatedModel, tempGroupList, firstRecord, sectionName);
                groupNameCounterSuffix++;
            }
        }
    }
}
