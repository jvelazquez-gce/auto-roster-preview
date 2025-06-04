using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.LastClassTogether
{
    public class LastClassTogetherHandler : ILastClassTogetherHandler
    {
        private Job _job;
        private IConfigurationRepository _configurationRepository;
        private Parameters _p;
        private CalcModel _calcModel;
        private readonly ILastClassTogetherCalculator _lastClassTogetherCalculator;
        private readonly IInCohortPreloadSectionHandler _inCohortPreloadSectionHandler;

        public LastClassTogetherHandler(
            Job job, 
            IConfigurationRepository configurationRepository,
            Parameters p,
            CalcModel calcModel,
            ILastClassTogetherCalculator lastClassTogetherCalculator,
            IInCohortPreloadSectionHandler inCohortPreloadSectionHandler)
        {
            _job = job;
            _configurationRepository = configurationRepository;
            _p = p;
            _calcModel = calcModel;
            _lastClassTogetherCalculator = lastClassTogetherCalculator;
            _inCohortPreloadSectionHandler = inCohortPreloadSectionHandler;
        }

        public void ProcessData()
        {
            _p.ClassesTogetherToGroupRuleList = _p.GetClassesTogetherToGroupRulesQuery.ExecuteQuery(new QueryInputParams());

            var courseIdsToGroupBy = _p.ClassesTogetherToGroupRuleList
                .Select(s => s.AdCourseID)
                .Distinct()
                .ToList();

            if (!courseIdsToGroupBy.Contains(_calcModel.CourseID)) return;

            var preLoadDnpRecordsToProcess =
                _inCohortPreloadSectionHandler.FindAndMarkRecordsAsLastClassTogetherCohortRemoveNonCohortsFromDbAndStudentCleanUp();

            ProcessRecords(preLoadDnpRecordsToProcess, _job, _configurationRepository);
        }

        private void ProcessRecords(List<PreLoadStudentSection> preLoadDnpRecordsToProcess,
            Job job, IConfigurationRepository configurationRepository)
        {
            var calculatedModel = new CalcModel();
            var groupList = preLoadDnpRecordsToProcess
                .Where(r => r.GroupTypeKey == Group.LAST_CLASS_TOGETHER_COHORT)
                .ToList();

            var noGroupList = preLoadDnpRecordsToProcess
                .Where(r => r.GroupTypeKey == Group.LAST_CLASS_TOGETHER_NO_COHORT)
                .ToList();

            if (!groupList.Any()) return;
            
            _lastClassTogetherCalculator.ProcessRecords(calculatedModel);

            // Any left over non group students will be handled as part of the normal ARB process which will put them in a O5xx section
            foreach (var rec in noGroupList)
            {
                rec.GroupTypeKey = Group.LAST_CLASS_TOGETHER_NO_COHORT_NO_EMPTY_SEAT_FOUND;
                rec.GroupNumber = Guid.Parse(LastClassGroupConstants.ONE_STUDENT_ONLY_COHORT_GROUP_NO_EMPTY_SEAT_FOUND_NUMBER);
            }
        }
    }
}
