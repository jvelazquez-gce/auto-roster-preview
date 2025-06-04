using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;

namespace Services.RulesEngine.GroupedStudents
{
    public class KeepClassTogetherCalculator : IKeepClassTogetherCalculator
    {
        private List<PreLoadStudentSection> _groupList;
        private Job _job;
        private Parameters _p;
        private CalcModel _calcModel;
        private IFillSectionHandler _fillSectionHandler;
        private readonly ICalculator _calculator;
        private readonly IMapRecordsAndAddToCalcModel _mapRecordsAndAddToCalcModel;
        private readonly IReCourseSectionHandler _reCourseSectionHandler;
        private readonly IReUseSectionHandler _reUseSectionHandler;

        public KeepClassTogetherCalculator(List<PreLoadStudentSection> groupList,
            Job job, 
            Parameters p,
            CalcModel calcModel,
            IFillSectionHandler fillSectionHandler,
            ICalculator calculator,
            IMapRecordsAndAddToCalcModel mapRecordsAndAddToCalcModel,
            IReCourseSectionHandler reCourseSectionHandler,
            IReUseSectionHandler reUseSectionHandler) 
        {
            _groupList = groupList;
            _job = job;
            _p = p;
            _calcModel = calcModel;
            _fillSectionHandler = fillSectionHandler;
            _calculator = calculator;
            _mapRecordsAndAddToCalcModel = mapRecordsAndAddToCalcModel;
            _reCourseSectionHandler = reCourseSectionHandler;
            _reUseSectionHandler = reUseSectionHandler;
        }

        public void ProcessRecords(CalcModel calculatedModel)
        {
            _reUseSectionHandler.FindSectionsToReUseIfPossibleAndAssignStudents();
            CombineNewCohortStudentsIntoSections(calculatedModel);
            _fillSectionHandler.FitNonCohortsLeftIntoEmptySeatsInDb();
            _reCourseSectionHandler.CreateCourseSectionsAndSaveThemAndLastClassGroupStudentSectionsToDb(calculatedModel, _p);
            SetUpLiveJobDataIfLiveJobIsRunning();
        }

        private void SetUpLiveJobDataIfLiveJobIsRunning()
        {
            if (_p.Job.StatusId != JobStatus.RUNNING_LIVE_JOB) return;

            var queryInputParams = new QueryInputParams() { AdCourseID = _calcModel.CourseID, StartDate = _calcModel.StartDate };
            var courseSections = _p.GetActiveClassGroupCourseSectionsToCreateQuery.ExecuteQuery(queryInputParams);
            var classGroupStudentSections = _p.GetClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);

            if (courseSections == null || !courseSections.Any() || classGroupStudentSections == null || !classGroupStudentSections.Any()) return;

            _calcModel.CourseSections.AddRange(courseSections);
            _calcModel.ClassGroupStudentSections.AddRange(classGroupStudentSections);
        }

        private void CombineNewCohortStudentsIntoSections(CalcModel calculatedModel)
        {
            var groupNameCounterSuffix =
                SectionCodeCalculator.GetNextRuleClassTogetherSectionNumberToCreate(_p, _calcModel.CourseID, _calcModel.StartDate, _calcModel.Rules.First());

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

            firstRecord.GroupCategory = ARBGroupCategory.RULES_CLASS_TOGETHER_COHORT;
            if (calculatedModel.TotalSectionsNeeded > 1)
            {
                var groupNameSuffix = _calcModel.Rules.First().SectionPrefix;
                _mapRecordsAndAddToCalcModel.BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(calculatedModel, 
                    firstRecord, tempGroupList, groupNameSuffix, ref groupNameCounterSuffix);
            }
            else if (calculatedModel.TotalSectionsNeeded == 1)
            {
                var sectionName = _calcModel.Rules.First().SectionPrefix + groupNameCounterSuffix;
                _mapRecordsAndAddToCalcModel.AddAllStudentsToModelInOneSection(calculatedModel, tempGroupList, firstRecord, sectionName);
                groupNameCounterSuffix++;
            }
        }
    }
}
