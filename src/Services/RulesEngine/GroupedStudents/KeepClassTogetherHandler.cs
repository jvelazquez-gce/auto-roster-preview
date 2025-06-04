using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.RulesEngine.GroupedStudents
{
    public class KeepClassTogetherHandler
    {
        private Job _job;
        private IConfigurationRepository _configurationRepository;
        private Parameters _p;
        private CalcModel _calcModel;
        private readonly IKeepClassTogetherCalculator _keepClassTogetherCalculator;
        private readonly IInGroupPreloadSectionHandler _inGroupPreloadSectionHandler;

        public KeepClassTogetherHandler(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, Job job, 
            IConfigurationRepository configurationRepository,
            Parameters p,
            CalcModel calcModel,
            IKeepClassTogetherCalculator keepClassTogetherCalculator, 
            IInGroupPreloadSectionHandler inGroupPreloadSectionHandler)
        {
            _job = job;
            _configurationRepository = configurationRepository;
            _p = p;
            _calcModel = calcModel;
            _keepClassTogetherCalculator = keepClassTogetherCalculator;
            _inGroupPreloadSectionHandler = inGroupPreloadSectionHandler;
        }

        public void ProcessData()
        {
            var rules = _p.GetActiveRulesByCourseIdQuery.ExecuteQuery(_p.QueryInputParams);

            if (!rules.Any()) return;

            //if (!_sectionsWithSameCourseAndStartDate.Any(w => w.AdProgramVersionID == 3315)) return;

            //if (!LastClassGroupConstants.DNP_COURSE_IDS_THAT_NEED_TO_BE_GROUPED.Contains(_calcModel.CourseID)) return;

            _calcModel.Rules = rules;
            var preLoadDnpRecordsToProcess = _inGroupPreloadSectionHandler
                .FindAndMarkRecordsAsInRuleGroupRemoveNotInRuleGroupFromDbAndStudentCleanUp();

            ProcessRecords(preLoadDnpRecordsToProcess, _job, _configurationRepository);
        }

        public void ProcessRecords(List<PreLoadStudentSection> preLoadRecordsToProcess,
            Job job, IConfigurationRepository configurationRepository)
        {
            var calculatedModel = new CalcModel();
            calculatedModel.Rules = _calcModel.Rules;
            var inRuleGroupList = preLoadRecordsToProcess
                .Where(r => r.GroupTypeKey == Group.IN_RULE_GROUP)
                .ToList();

            var notInRuleGroupList = preLoadRecordsToProcess
                .Where(r => r.GroupTypeKey == Group.NOT_IN_RULE_GROUP)
                .ToList();

            _keepClassTogetherCalculator.ProcessRecords(calculatedModel);

            // Any left over non group students will be handled as part of the normal ARB process which will put them in a O5xx section
            foreach (var rec in notInRuleGroupList)
            {
                rec.GroupTypeKey = Group.NOT_IN_RULE_GROUP_NO_EMPTY_SEAT_FOUND;
                //rec.GroupNumber = Guid.Parse(LastClassGroupConstants.ONE_STUDENT_ONLY_COHORT_GROUP_NO_EMPTY_SEAT_FOUND_NUMBER);
            }
        }
    }
}
