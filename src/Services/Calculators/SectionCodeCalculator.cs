using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.Calculators
{
    public class SectionCodeCalculator
    {
        public string GetCohortNameAndAssignGroupCategory(int groupType, PreLoadStudentSection firtRecord)
        {
            var groupName = GeneralStatus.COHORT_NAME_PREFIX + firtRecord.ProgramStartMonthAndDay;
            switch (groupType)
            {
                case Group.EXCLUSIVE_COHORT:
                    firtRecord.GroupCategory = ARBGroupCategory.EXCLUSIVE;
                    return GeneralStatus.EXCLUSIVE_GROUP_NAME_PREFIX + firtRecord.ProgramStartMonthAndDay;
                case Group.INCLUSIVE_COHORT:
                    firtRecord.GroupCategory = ARBGroupCategory.INCLUSIVE;
                    return GeneralStatus.INCLUSIVE_GROUP_NAME_PREFIX + firtRecord.ProgramStartMonthAndDay;
                case Group.LAST_CLASS_TOGETHER_COHORT:
                    firtRecord.GroupCategory = ARBGroupCategory.LAST_CLASS_TOGETHER_COHORT;
                    return GeneralStatus.LAST_CLASS_TOGETHER_COHORT_PREFIX;
                case Group.FRIEND_COHORT:
                    firtRecord.GroupCategory = ARBGroupCategory.FRIEND;
                    return GeneralStatus.FRIEND_GROUP_NAME_PREFIX;
                default:
                    firtRecord.GroupCategory = ARBGroupCategory.GENERAL;
                    return GeneralStatus.NOCOHORT_NOLOCATION_NAME_PREFIX;
            }
        }

        public List<string> CreateGroupNames(int numberOfGroups, string mainGroupName, ref int baseCounter)
        {
            // i.e.,: start: baseCounter = 510; originalCounter = 510, numberOfGroups = 2;
            // i.e.,: end: baseCounter = 512. O510 and O511 created.
            var groupNames = new List<string>();
            var originalCounter = baseCounter;
            while (baseCounter < originalCounter + numberOfGroups)
            {
                groupNames.Add(mainGroupName + baseCounter);
                baseCounter++;
            }
            return groupNames;
        }

        public static int GetNextLastClassTogetherSectionNumberToCreate(Parameters p, int courseId, DateTime startDate)
        {
            var queryInputParams = new QueryInputParams() { AdCourseID = courseId, StartDate = startDate };
            var lastClassGroupCourseSections = p.GetActiveLastClassGroupCourseSectionsToCreateQuery.ExecuteQuery(queryInputParams);
            var sectionCodes = lastClassGroupCourseSections
                .Select(r => r.SectionCode)
                .Distinct()
                .ToList();

            var firstTwoCharactersInARBCreatedSections = GeneralStatus.LAST_CLASS_TOGETHER_COHORT_PREFIX 
                                                         + GeneralStatus.LAST_CLASS_TOGETHER_BASE_COUNTER_FIRST_DIGIT_STRING;

            var codesToExclude = sectionCodes
                .Where(s => !s.StartsWith(firstTwoCharactersInARBCreatedSections))
                .ToList();

            codesToExclude.ForEach(s => sectionCodes.Remove(s));

            var sectionNumbers = new List<int>();
            foreach (var sectionCode in sectionCodes)
            {
                try
                {
                    sectionNumbers.Add(Convert.ToInt32(sectionCode.Remove(0, 1)));
                }
                catch (Exception) { }
            }

            sectionNumbers = sectionNumbers
                .OrderByDescending(s => s)
                .ToList();

            var lastNumber = sectionNumbers.FirstOrDefault();

            if (lastNumber > 0) return ++lastNumber;

            return GeneralStatus.BASE_COUNTER;
        }

        public static int GetNextRuleClassTogetherSectionNumberToCreate(Parameters p, int courseId, DateTime startDate, Rule rule)
        {
            var queryInputParams = new QueryInputParams() { AdCourseID = courseId, StartDate = startDate };
            var classGroupStudentSection = p.GetClassGroupStudentsToTransferQuery.ExecuteQuery(queryInputParams);

            var sectionCodes = classGroupStudentSection
                .Select(r => r.SectionCode)
                .Distinct()
                .ToList();

            var firstTwoCharactersInARBCreatedSections = $"{rule.SectionPrefix}{rule.SectionBaseCounterFirstDigitString}";

            var codesToExclude = sectionCodes
                .Where(s => !s.StartsWith(firstTwoCharactersInARBCreatedSections))
                .ToList();

            codesToExclude.ForEach(s => sectionCodes.Remove(s));

            var sectionNumbers = new List<int>();
            foreach (var sectionCode in sectionCodes)
                try
                {
                    sectionNumbers.Add(Convert.ToInt32(sectionCode.Remove(0, 1)));
                }
                catch (Exception) { }

            sectionNumbers = sectionNumbers
                .OrderByDescending(s => s)
                .ToList();

            var lastNumber = sectionNumbers.FirstOrDefault();

            if (lastNumber > 0) return ++lastNumber;

            return GeneralStatus.BASE_COUNTER;
        }

    }
}
