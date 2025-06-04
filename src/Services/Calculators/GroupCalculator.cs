using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Domain.Entities;
using Infrastructure.Utilities;

namespace Services.Calculators
{
    public class GroupCalculator : IGroupCalculator
    {
        private readonly ICalculator _calculator;
        private readonly IConfigInstance _configInstance;
        private readonly IErrorMitigation _errorMitigation;

        public GroupCalculator(ICalculator calculator, IConfigInstance configInstance, IErrorMitigation errorMitigation)
        {
            _calculator = calculator;
            _configInstance = configInstance;
            _errorMitigation = errorMitigation;
        }

        public CalcModel GetCalculatedSectionsModel(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, 
            int groupType, 
            int groupNameCounterSuffix, 
            Job job, 
            IConfigurationRepository configurationRepository)
        {
            var calculatedModel = new CalcModel();
            var groupList = sectionsWithSameCourseAndStartDate
                .Where(r => r.GroupTypeKey == groupType)
                .ToList();

            if (groupList.Count == 0) return calculatedModel;

            var distinctGroupIds = groupList.Select(r => r.GroupNumber).Distinct().ToList();
            var sectionCodeCalculator = new SectionCodeCalculator();
            foreach (var groupNumber in distinctGroupIds)
            {
                var tempGroupList = groupList.Where(l => l.GroupNumber == groupNumber).ToList();
                var firstRecord = tempGroupList.FirstOrDefault();
                _calculator.CalculateSectionsNeeded(firstRecord, tempGroupList, calculatedModel);
                string groupNameSuffix;
                if (calculatedModel.TotalSectionsNeeded > 1)
                {
                    groupNameSuffix = sectionCodeCalculator.GetCohortNameAndAssignGroupCategory(groupType, firstRecord);
                    _calculator.BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(calculatedModel, firstRecord, tempGroupList, groupNameSuffix, ref groupNameCounterSuffix, job);
                }
                else if (calculatedModel.TotalSectionsNeeded == 1)
                {
                    if (tempGroupList.Count >= _configInstance.GetCohortMinSize())
                    {
                        groupNameSuffix = sectionCodeCalculator.GetCohortNameAndAssignGroupCategory(groupType, firstRecord) + groupNameCounterSuffix.ToString();
                        _calculator.AddAllStudentsToModelInOneSection(calculatedModel, tempGroupList, firstRecord, groupNameSuffix, job);
                        groupNameCounterSuffix++;
                    }
                    else if (tempGroupList.Count < _configInstance.GetCohortMinSize() && tempGroupList.Count > 0)
                        _errorMitigation.LessThanMinAllowedCohortRecords(tempGroupList, job, sectionsWithSameCourseAndStartDate);
                }
            }
            return calculatedModel;
        }

        public void CombineSmallestToBiggestGroupSectionsIfPossibleToCreateTheLeastAmountOfSections(CalcModel calculatedModel, List<PreviewStudentSection> groupListToCombine, Job job)
        {
            if (groupListToCombine.Count == 0) return;

            var uniqueSectionList = _calculator.GetDistinctSectionListOrderByHighestStudentsInSection(groupListToCombine);
            while (uniqueSectionList.Count > 1)
            {
                var biggestSection = uniqueSectionList.First();
                var smallestSection = uniqueSectionList.Last();

                while (biggestSection.TotalEmptySeats >= smallestSection.TotalStudentsInSection && uniqueSectionList.Count > 1)
                {
                    TransferStudentSectionsByAmount(smallestSection.SectionCode, biggestSection.SectionCode, smallestSection.TotalStudentsInSection, calculatedModel, biggestSection.GroupCategory, job);
                    uniqueSectionList.Remove(smallestSection);
                    biggestSection.TotalStudentsInSection += smallestSection.TotalStudentsInSection;
                    biggestSection.TotalEmptySeats -= smallestSection.TotalStudentsInSection;
                    smallestSection = uniqueSectionList.Last();
                }
                uniqueSectionList.Remove(biggestSection);
            }
        }

        public void TransferStudentSectionsByAmount(string sectionCodeToTakeStudentsFrom, string sectionCodeToAddStudentsInto, int amount, CalcModel calculatedModel, int newGroupCategory, Job job)
        {
            var studentRecordsToRemoveUpdateAndAdd = calculatedModel.PreviewStudentRecords
                .Where(r => r.SectionCode == sectionCodeToTakeStudentsFrom)
                .Take(amount)
                .ToList();

            foreach(var studentRecord in studentRecordsToRemoveUpdateAndAdd)
            {
                var oldSectionCode = studentRecord.SectionCode;
                calculatedModel.PreviewStudentRecords.Remove(studentRecord);
                studentRecord.SectionCode = sectionCodeToAddStudentsInto;
                studentRecord.GroupCategory = newGroupCategory;
                calculatedModel.PreviewStudentRecords.Add(studentRecord);
            }
        }

        public List<PreviewStudentSection> NonExclusiveCohortPreviewStudentRecords(CalcModel calculatedModel)
        {
            return calculatedModel.PreviewStudentRecords.Where(r => Group.NON_EXCLUSIVE_COHORT_CATEGORY_LIST.Contains(r.GroupCategory)).ToList();
        }
    }
}
