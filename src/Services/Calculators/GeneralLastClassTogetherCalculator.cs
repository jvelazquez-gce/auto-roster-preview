using System.Collections.Generic;
using System.Linq;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.Calculators
{
    public class GeneralLastClassTogetherCalculator : IGeneralLastClassTogetherCalculator
    {
        private readonly ICalculator _calculator;
        private readonly INoGroupCalculator _noGroupCalculator;

        public GeneralLastClassTogetherCalculator(ICalculator calculator, INoGroupCalculator noGroupCalculator)
        {
            _calculator = calculator;
            _noGroupCalculator = noGroupCalculator;
        }

        public void CombineStudentsThatTookLastClassTogetherForAllHomogeneousSections(CalcModel calculatedModel, Job job)
        {
            var allNonCohortSectionsList = _noGroupCalculator.NonCohortPreviewStudentRecords(calculatedModel.PreviewStudentRecords)
                .Select(r => r.SectionCode)
                .Distinct()
                .ToList();

            while (allNonCohortSectionsList.Count > 0)
            {
                var sectionCode = allNonCohortSectionsList.First();
                var homogeneousSectionList = GetListOfHomogeneousSectionListForTheProvidedSectionCode(sectionCode, allNonCohortSectionsList, job);
                var listToUpdate = new List<PreviewStudentSection>();

                if (!homogeneousSectionList.Any()) continue;

                listToUpdate = calculatedModel.PreviewStudentRecords
                    .Where(r => homogeneousSectionList.Contains(r.SectionCode))
                    .ToList();

                CombineStudentsThatTookLastClassTogether(listToUpdate, calculatedModel, job);
            }
        }

        public List<string> GetListOfHomogeneousSectionListForTheProvidedSectionCode(string sectionCode, List<string> allSectionsList, Job job)
        {
            if (allSectionsList.Count < 1) return new List<string>();

            var sectionCodeWithOutCounter = sectionCode.Substring(0, sectionCode.Length - 1);
            var homogeneousSectionList = allSectionsList
                .Where(r => r.Contains(sectionCodeWithOutCounter))
                .ToList();

            homogeneousSectionList.ForEach(r => allSectionsList.Remove(r));

            return homogeneousSectionList;
        }

        private void CombineStudentsThatTookLastClassTogether(List<PreviewStudentSection> homogeneousListToUpdate, CalcModel calculatedModel, Job job)
        {
            var listOfGroupsThatTookLastClassTogether = GetListOfStudentsThatTookClassesTogetherOrderByHighest(homogeneousListToUpdate);
            if (listOfGroupsThatTookLastClassTogether.Count == 0) return;
            var listOfSectionBeforeLastClassTogetherCalculation = _calculator.GetDistinctSectionListOrderByHighestStudentsInSection(homogeneousListToUpdate);
            var originalListOfSectionBeforeLastClassTogetherCalculation = listOfSectionBeforeLastClassTogetherCalculation.Select(i => (SectionTotals)i.Clone()).ToList();
            var originalHomogeneousList = homogeneousListToUpdate;
            foreach (var record in homogeneousListToUpdate)
            {
                calculatedModel.PreviewStudentRecords.Remove(record);
            }
            var groupedStudentsBySection = new List<PreviewStudentSection>();

            TryToMoveLastClassTogetherGroupsIntoExistingSectionsUpToTotalStudentsInSections(groupedStudentsBySection, listOfGroupsThatTookLastClassTogether,
                listOfSectionBeforeLastClassTogetherCalculation, homogeneousListToUpdate, job);

            groupedStudentsBySection.ForEach(r => homogeneousListToUpdate.Remove(r));
            FillSectionsToTheOriginalValuesWithTheLeftOverStudents(groupedStudentsBySection, homogeneousListToUpdate, originalListOfSectionBeforeLastClassTogetherCalculation, job);
            VerifyResultsAndApplyThem(originalListOfSectionBeforeLastClassTogetherCalculation, groupedStudentsBySection, calculatedModel, originalHomogeneousList, job);
        }

        public bool VerifyResultsAndApplyThem(List<SectionTotals> originalListOfSectionBeforeLastClassTogetherCalculation, List<PreviewStudentSection> updatedListOfSections,
            CalcModel calculatedModel, List<PreviewStudentSection> originalHomogeneousList, Job job)
        {
            var listOfLastClassTogetherSections = _calculator.GetDistinctSectionListOrderByHighestStudentsInSection(updatedListOfSections);
            originalListOfSectionBeforeLastClassTogetherCalculation = originalListOfSectionBeforeLastClassTogetherCalculation.OrderByDescending(s => s.TotalStudentsInSection).ToList();

            if (listOfLastClassTogetherSections.Count != originalListOfSectionBeforeLastClassTogetherCalculation.Count)
            {
                foreach (var record in originalHomogeneousList)
                {
                    calculatedModel.PreviewStudentRecords.Add(record);
                }
                return false;
            }

            while (listOfLastClassTogetherSections.Count > 0 && originalListOfSectionBeforeLastClassTogetherCalculation.Count > 0)
            {
                var lastClassTogetherSection = listOfLastClassTogetherSections.First();
                var section = originalListOfSectionBeforeLastClassTogetherCalculation.First();
                listOfLastClassTogetherSections.Remove(lastClassTogetherSection);
                originalListOfSectionBeforeLastClassTogetherCalculation.Remove(section);
                if (lastClassTogetherSection.TotalStudentsInSection == section.TotalStudentsInSection) continue;
                foreach (var record in originalHomogeneousList)
                {
                    calculatedModel.PreviewStudentRecords.Add(record);
                }
                return false;
            }

            foreach (var record in updatedListOfSections)
            {
                calculatedModel.PreviewStudentRecords.Add(record);
            }
            return true;
        }

        public void FillSectionsToTheOriginalValuesWithTheLeftOverStudents(List<PreviewStudentSection> groupedStudentsBySection, List<PreviewStudentSection> studentsLeftList,
            List<SectionTotals> originalListOfSectionBeforeLastClassTogetherCalculation, Job job)
        {
            foreach (var originalSection in originalListOfSectionBeforeLastClassTogetherCalculation)
            {
                var updatedStudentRecords = groupedStudentsBySection.Where(r => r.SectionCode == originalSection.SectionCode).ToList();
                var updatedStudentRecordsCount = updatedStudentRecords.Count;
                if (originalSection.TotalStudentsInSection > updatedStudentRecordsCount)
                {
                    var tempList = studentsLeftList.Take(originalSection.TotalStudentsInSection - updatedStudentRecordsCount).ToList();
                    foreach (var record in tempList)
                    {
                        studentsLeftList.Remove(record);
                        record.SectionCode = originalSection.SectionCode;
                        record.GroupCategory = originalSection.GroupCategory;
                        groupedStudentsBySection.Add(record);
                    }
                }
                else if (originalSection.TotalStudentsInSection < updatedStudentRecordsCount)
                {
                    break;
                }
            }
        }

        private bool IsThereMoreDataToProcess(ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<SectionTotals> listOfSectionsBeforeLastClassTogetherCalculation,
            ref LastClassTogetherGroup biggestClassTogetherGroup, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether)
        {
            // Error checking
            if (biggestClassTogetherGroup == null && listOfGroupsThatTookLastClassTogether.Count == 0) return false;
            if (sectionWithSeatsAvailableToPutGroupStudentsInto == null && listOfSectionsBeforeLastClassTogetherCalculation.Count == 0) return false;

            // Find if there is a perfect fit for the biggest class together group
            if (biggestClassTogetherGroup == null)
            {
                biggestClassTogetherGroup = listOfGroupsThatTookLastClassTogether.First();
                listOfGroupsThatTookLastClassTogether.Remove(biggestClassTogetherGroup);
            }
            else
            {
                listOfGroupsThatTookLastClassTogether.Add(biggestClassTogetherGroup);
                listOfGroupsThatTookLastClassTogether = listOfGroupsThatTookLastClassTogether.OrderByDescending(r => r.TotalStudents).ToList();
                biggestClassTogetherGroup = listOfGroupsThatTookLastClassTogether.First();
                listOfGroupsThatTookLastClassTogether.Remove(biggestClassTogetherGroup);
            }

            if (sectionWithSeatsAvailableToPutGroupStudentsInto != null)
            {
                listOfSectionsBeforeLastClassTogetherCalculation.Add(sectionWithSeatsAvailableToPutGroupStudentsInto);
                listOfSectionsBeforeLastClassTogetherCalculation = listOfSectionsBeforeLastClassTogetherCalculation
                    .OrderByDescending(r => r.TotalStudentsInSection)
                    .ToList();
            }

            var totalStudentsInBiggestClassTogetherGroup = biggestClassTogetherGroup.TotalStudents;
            sectionWithSeatsAvailableToPutGroupStudentsInto = listOfSectionsBeforeLastClassTogetherCalculation
                .FirstOrDefault(r => r.TotalStudentsInSection == totalStudentsInBiggestClassTogetherGroup);

            if (sectionWithSeatsAvailableToPutGroupStudentsInto != null)
            {
                listOfSectionsBeforeLastClassTogetherCalculation.Remove(sectionWithSeatsAvailableToPutGroupStudentsInto);
                return true;
            }

            sectionWithSeatsAvailableToPutGroupStudentsInto = listOfSectionsBeforeLastClassTogetherCalculation.First();
            listOfSectionsBeforeLastClassTogetherCalculation.Remove(sectionWithSeatsAvailableToPutGroupStudentsInto);
            return true;
        }

        public void TryToMoveLastClassTogetherGroupsIntoExistingSectionsUpToTotalStudentsInSections(List<PreviewStudentSection> groupedStudentsBySection,
            List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether, List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation,
            List<PreviewStudentSection> homogeneousListToUpdate, Job job)
        {
            if (!listOfGroupsThatTookLastClassTogether.Any() || !listOfSectionBeforeLastClassTogetherCalculation.Any()) return;

            SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto = null;
            LastClassTogetherGroup biggestClassTogetherGroup = null;
            var done = !IsThereMoreDataToProcess(ref sectionWithSeatsAvailableToPutGroupStudentsInto, ref listOfSectionBeforeLastClassTogetherCalculation, ref biggestClassTogetherGroup, ref listOfGroupsThatTookLastClassTogether);

            while (!done)
            {
                if (sectionWithSeatsAvailableToPutGroupStudentsInto.TotalStudentsInSection == biggestClassTogetherGroup.TotalStudents)
                {
                    BiggestSectionIsEqualToBiggestClassTogetherGroup(ref biggestClassTogetherGroup, groupedStudentsBySection, ref sectionWithSeatsAvailableToPutGroupStudentsInto,
                        ref listOfGroupsThatTookLastClassTogether, ref listOfSectionBeforeLastClassTogetherCalculation, ref done);
                }
                else if (sectionWithSeatsAvailableToPutGroupStudentsInto.TotalStudentsInSection > biggestClassTogetherGroup.TotalStudents)
                {
                    BiggestSectionIsGreaterThanTheBiggestClassTogetherGroup(ref biggestClassTogetherGroup, groupedStudentsBySection, ref sectionWithSeatsAvailableToPutGroupStudentsInto,
                        ref listOfGroupsThatTookLastClassTogether, ref done, ref listOfSectionBeforeLastClassTogetherCalculation);
                }
                else if (sectionWithSeatsAvailableToPutGroupStudentsInto.TotalStudentsInSection < biggestClassTogetherGroup.TotalStudents)
                {
                    BiggestSectionIsLessThanTheBiggestClassTogetherGroup(ref biggestClassTogetherGroup, groupedStudentsBySection, ref sectionWithSeatsAvailableToPutGroupStudentsInto,
                        ref listOfGroupsThatTookLastClassTogether, ref listOfSectionBeforeLastClassTogetherCalculation, homogeneousListToUpdate, ref done);
                }
            }
        }

        public void BiggestSectionIsEqualToBiggestClassTogetherGroup(ref LastClassTogetherGroup biggestClassTogetherGroup, List<PreviewStudentSection> groupedStudentsBySection,
            ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether, ref List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation,
            ref bool done)
        {
            foreach (var record in biggestClassTogetherGroup.StudentRecords)
            {
                record.SectionCode = sectionWithSeatsAvailableToPutGroupStudentsInto.SectionCode;
                record.GroupCategory = sectionWithSeatsAvailableToPutGroupStudentsInto.GroupCategory;
                groupedStudentsBySection.Add(record);
            }

            sectionWithSeatsAvailableToPutGroupStudentsInto = null;
            biggestClassTogetherGroup = null;
            done = !IsThereMoreDataToProcess(ref sectionWithSeatsAvailableToPutGroupStudentsInto, ref listOfSectionBeforeLastClassTogetherCalculation, ref biggestClassTogetherGroup, ref listOfGroupsThatTookLastClassTogether);
        }

        public void BiggestSectionIsGreaterThanTheBiggestClassTogetherGroup(ref LastClassTogetherGroup biggestClassTogetherGroup, List<PreviewStudentSection> groupedStudentsBySection,
            ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether, ref bool done,
            ref List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation)
        {
            foreach (var record in biggestClassTogetherGroup.StudentRecords)
            {
                record.SectionCode = sectionWithSeatsAvailableToPutGroupStudentsInto.SectionCode;
                record.GroupCategory = sectionWithSeatsAvailableToPutGroupStudentsInto.GroupCategory;
                groupedStudentsBySection.Add(record);
            }
            sectionWithSeatsAvailableToPutGroupStudentsInto.TotalStudentsInSection -= biggestClassTogetherGroup.TotalStudents;

            biggestClassTogetherGroup = null;
            done = !IsThereMoreDataToProcess(ref sectionWithSeatsAvailableToPutGroupStudentsInto, ref listOfSectionBeforeLastClassTogetherCalculation, ref biggestClassTogetherGroup, ref listOfGroupsThatTookLastClassTogether);
        }

        public void BiggestSectionIsLessThanTheBiggestClassTogetherGroup(ref LastClassTogetherGroup biggestClassTogetherGroup,
            List<PreviewStudentSection> groupedStudentsBySection, ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether,
            ref List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation, List<PreviewStudentSection> homogeneousListToUpdate, ref bool done)
        {
            if (listOfSectionBeforeLastClassTogetherCalculation.Count > 0)
            {
                if (listOfSectionBeforeLastClassTogetherCalculation[0].TotalStudentsInSection >= biggestClassTogetherGroup.TotalStudents)
                {
                    sectionWithSeatsAvailableToPutGroupStudentsInto = listOfSectionBeforeLastClassTogetherCalculation.First();
                    listOfSectionBeforeLastClassTogetherCalculation.Remove(sectionWithSeatsAvailableToPutGroupStudentsInto);
                    return;
                }
            }

            var listToRemoveFromBiggestClassTogetherGroup = new List<PreviewStudentSection>();
            foreach (var record in biggestClassTogetherGroup.StudentRecords.Take(sectionWithSeatsAvailableToPutGroupStudentsInto.TotalStudentsInSection))
            {
                record.SectionCode = sectionWithSeatsAvailableToPutGroupStudentsInto.SectionCode;
                record.GroupCategory = sectionWithSeatsAvailableToPutGroupStudentsInto.GroupCategory;
                groupedStudentsBySection.Add(record);
                listToRemoveFromBiggestClassTogetherGroup.Add(record);
            }
            biggestClassTogetherGroup.TotalStudents -= sectionWithSeatsAvailableToPutGroupStudentsInto.TotalStudentsInSection;
            foreach (var record in listToRemoveFromBiggestClassTogetherGroup)
                biggestClassTogetherGroup.StudentRecords.Remove(record);

            sectionWithSeatsAvailableToPutGroupStudentsInto = null;
            done = !IsThereMoreDataToProcess(ref sectionWithSeatsAvailableToPutGroupStudentsInto, ref listOfSectionBeforeLastClassTogetherCalculation, ref biggestClassTogetherGroup, ref listOfGroupsThatTookLastClassTogether);
        }

        public List<LastClassTogetherGroup> GetListOfStudentsThatTookClassesTogetherOrderByHighest(List<PreviewStudentSection> listToUpdate)
        {
            var listOfUniqueLastClassesTaken = listToUpdate.Select(r => r.LastAdClassSchedIDTaken).Where(r => r != null).Distinct().ToList();
            var listOfGroupsThatTookLastClassTogether = new List<LastClassTogetherGroup>();
            foreach (var lastClassId in listOfUniqueLastClassesTaken)
            {
                var listOfStudentsThatTookTheLastClassTogether = listToUpdate.Where(r => r.LastAdClassSchedIDTaken == lastClassId).ToList();
                if (!listOfStudentsThatTookTheLastClassTogether.Any()) continue;

                var groupThatTookLastClassTogether = new LastClassTogetherGroup
                {
                    LastClassTakenId = (int)lastClassId,
                    TotalStudents = listOfStudentsThatTookTheLastClassTogether.Count,
                    StudentRecords = listOfStudentsThatTookTheLastClassTogether
                };
                listOfGroupsThatTookLastClassTogether.Add(groupThatTookLastClassTogether);
            }
            return listOfGroupsThatTookLastClassTogether.OrderByDescending(r => r.TotalStudents).ToList();
        }
    }
}
