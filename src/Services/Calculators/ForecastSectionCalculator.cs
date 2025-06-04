using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;
using AutoMapper;

namespace Services.Calculators
{
    public class ForecastSectionCalculator : IForecastSectionCalculator
    {
        private readonly IMapper _mapper;
        private readonly Parameters _p;
        private readonly ICalculator _calculator;
        private readonly INoGroupCalculator _noGroupCalculator;

        public ForecastSectionCalculator(IMapper mapper, Parameters p, ICalculator calculator, INoGroupCalculator noGroupCalculator)
        {
            _p = p;
            _mapper = mapper;
            _calculator = calculator;
            _noGroupCalculator = noGroupCalculator;
        }

        public int GetForeCastSectionsNeeded(PreviewStudentSection firstRecord, CalcModel calculatedModel, Job job)
        {
            if (!firstRecord.ForcastedNumberOfStudents.HasValue || firstRecord.ForcastedNumberOfStudents <= 0) return 0;

            var numberOfGeneralAndFriendStudentsRegistered = GetTotalGeneralOrFriendStudentsRegistered(calculatedModel);
            var numberOfEmptySitsInExistingSections = GetNumberOfEmptySeatsInExistingSections(calculatedModel, numberOfGeneralAndFriendStudentsRegistered,
                (int)firstRecord.ForcastedNumberOfStudents, job);

            var totalForecastedStudentsWithoutSections = (double)(firstRecord.ForcastedNumberOfStudents - (numberOfGeneralAndFriendStudentsRegistered + numberOfEmptySitsInExistingSections));

            if (calculatedModel.MaxStudentsPerSection <= 0 || totalForecastedStudentsWithoutSections <= 0) return 0;

            var sectionsNeeded = (int)Math.Ceiling(totalForecastedStudentsWithoutSections / (double)calculatedModel.MaxStudentsPerSection);
            return sectionsNeeded <= 0 ? 0 : sectionsNeeded;
        }

        public int GetTotalGeneralOrFriendStudentsRegistered(CalcModel calculatedModel)
        {
            return calculatedModel.PreviewStudentRecords
                .Where(r => (!r.GroupTypeKey.HasValue || r.GroupTypeKey == 0) || r.GroupTypeKey == Group.FRIEND_COHORT)
                .ToList()
                .Count();
        }

        public int GetNextSectionNumberToCreate(CalcModel calculatedModel)
        {
            var sectionCodes = _noGroupCalculator.NonCohortPreviewStudentRecords(calculatedModel.PreviewStudentRecords)
                .Select(r => r.SectionCode)
                .Distinct()
                .ToList();

            var sectionCodesNotReUsed = calculatedModel.CourseSectionsThatWereNotReUsed.Where(s => s.GroupCategory == ARBGroupCategory.GENERAL)
                .Select(s => s.SectionCode)
                .Distinct()
                .ToList();

            foreach(var sectionCode in sectionCodesNotReUsed)
                if (!sectionCodes.Contains(sectionCode))
                    sectionCodes.Add(sectionCode);

            var firstTwoCharactersInARBCreatedSections = GeneralStatus.NOCOHORT_NOLOCATION_NAME_PREFIX + GeneralStatus.NOCOHORT_BASE_COUNTER_FIRST_DIGIT_STRING;

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

        public int GetNumberOfEmptySeatsInExistingSections(CalcModel calculatedModel, int numberOfGeneralAndFriendStudentsRegistered, 
                                            int numberOfForecastedStudents, Job job)
        {
            var listOfGeneralSectionsNotReUsed = calculatedModel.CourseSectionsThatWereNotReUsed
                .Where(s => s.GroupCategory == ARBGroupCategory.GENERAL && s.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER)
                .ToList();

            var numberOfEmptySeatsInNonGroupSections = GetNumberOfEmptySitsInGeneralSections(calculatedModel);
            var numberOfEmptySeatsInInclusiveSections = GetNumberOfEmptySeatsInARBInclusiveSections(calculatedModel);
            var numberOfEmptySeatsInSectionsThatWereNotReUsed = GetTotalNumberOfSitsInList(listOfGeneralSectionsNotReUsed);
            var totalEmptySeats = numberOfEmptySeatsInNonGroupSections + numberOfEmptySeatsInInclusiveSections + numberOfEmptySeatsInSectionsThatWereNotReUsed;

            MarkEmptyExistingSectionsThatCanBeReUsed(calculatedModel, numberOfGeneralAndFriendStudentsRegistered, numberOfEmptySeatsInNonGroupSections,
                numberOfEmptySeatsInInclusiveSections, numberOfForecastedStudents, listOfGeneralSectionsNotReUsed, job);

            return totalEmptySeats;
        }

        private void MarkEmptyExistingSectionsThatCanBeReUsed(CalcModel calculatedModel, int numberOfGeneralAndFriendStudentsRegistered, 
            int numberOfEmptySitsInNonGroupSections, int numberOfEmptySeatsInInclusiveSections, int numberOfForecastedStudents, 
            List<CourseSection> listOfGeneralSectionsNotReUsed, Job job)
        {
            var totalForecastedStudentsWithoutSeats = 
                numberOfForecastedStudents - (numberOfGeneralAndFriendStudentsRegistered + numberOfEmptySitsInNonGroupSections + numberOfEmptySeatsInInclusiveSections);

            if (totalForecastedStudentsWithoutSeats <= 0) return;

            var sectionsReUsed = new List<CourseSection>();
            foreach (var section in listOfGeneralSectionsNotReUsed)
            {
                totalForecastedStudentsWithoutSeats -= section.MaximumNumberOfStudents;
                calculatedModel.CourseSectionsThatWereNotReUsed.Remove(section);
                section.ForecastSection = true;
                sectionsReUsed.Add(section);
                calculatedModel.CourseSectionsThatWereNotReUsed.Add(section);
                if (totalForecastedStudentsWithoutSeats <= 0) break;
            }
            _p.UpdateCourseSectionCommand.Execute(sectionsReUsed);
        }

        public int GetNumberOfEmptySitsInGeneralSections(CalcModel calculatedModel)
        {
            var firstGeneralSection = calculatedModel.CourseSections
                .Where(s => s.GroupCategory == ARBGroupCategory.GENERAL)
                .FirstOrDefault(s => s.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER);

            if (firstGeneralSection == null) return 0;

            int numberOfGeneralSections = 0, maxStudentsPerSection = 0, numberOfEmptySeatsInGeneralSections = 0;

            numberOfGeneralSections = calculatedModel.CourseSections
                .Where(s => s.GroupCategory == ARBGroupCategory.GENERAL)
                .Count(s => s.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER);

            maxStudentsPerSection = firstGeneralSection.MaximumNumberOfStudents;

            var totalStudentsInSections = calculatedModel.PreviewStudentRecords
                .Where(r => r.GroupCategory == ARBGroupCategory.GENERAL)
                .Count(r => r.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER);

            numberOfEmptySeatsInGeneralSections = (numberOfGeneralSections * maxStudentsPerSection) - totalStudentsInSections;

            return numberOfEmptySeatsInGeneralSections;
        }

        public int GetNumberOfEmptySeatsInARBInclusiveSections(CalcModel calculatedModel)
        {
            var firstInclusiveSection = calculatedModel.CourseSections
                .Where(s => s.GroupCategory == ARBGroupCategory.INCLUSIVE)
                .FirstOrDefault(s => s.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER);

            if (firstInclusiveSection == null) return 0;

            int numberOfInclusiveSections = 0, maxStudentsPerSection = 0, numberOfEmptySitsInInclusiveSections = 0;

            numberOfInclusiveSections = calculatedModel.CourseSections
                .Where(s => s.GroupCategory == ARBGroupCategory.INCLUSIVE)
                .Count(s => s.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER);

            maxStudentsPerSection = firstInclusiveSection.MaximumNumberOfStudents;

            var totalStudentsRegistered = calculatedModel.PreviewStudentRecords
                .Where(r => r.GroupCategory == ARBGroupCategory.INCLUSIVE)
                .Count(r => r.SectionCode != GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER);

            numberOfEmptySitsInInclusiveSections = (numberOfInclusiveSections * maxStudentsPerSection) - totalStudentsRegistered;

            return numberOfEmptySitsInInclusiveSections;
        }

        public PreviewStudentSection GetForecastRecord(CalcModel calculatedModel)
        {
            var firstRecord = new PreviewStudentSection();
            if (calculatedModel.CourseStartDateListOnlyHasClassesWithZeroStudents)
            {
                var record = calculatedModel.PreloadRecordsWithoutStudentIDs
                    .FirstOrDefault(r => (!r.GroupTypeKey.HasValue || r.GroupTypeKey == 0) || r.GroupTypeKey == Group.FRIEND_COHORT);

                if (record != null)
                {
                    _mapper.Map<PreLoadStudentSection, PreviewStudentSection>(record, firstRecord);
                    _calculator.CalculateSectionsNeeded(record, new List<PreLoadStudentSection>(), calculatedModel);
                }
                else
                    firstRecord = null;
            }
            else
            {
                firstRecord = calculatedModel.PreviewStudentRecords
                    .FirstOrDefault(r => (!r.GroupTypeKey.HasValue || r.GroupTypeKey == 0) || r.GroupTypeKey == Group.FRIEND_COHORT);
            }
            return firstRecord;
        }

        private int GetTotalNumberOfSitsInList(List<CourseSection> list)
        {
            if (list == null || list.Count == 0) return 0;
            var totalSits = 0;
            list.ForEach(s => totalSits += s.MaximumNumberOfStudents);
            return totalSits;
        }
    }
}
