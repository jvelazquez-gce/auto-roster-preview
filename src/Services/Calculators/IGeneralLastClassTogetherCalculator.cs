using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface IGeneralLastClassTogetherCalculator
    {
        void CombineStudentsThatTookLastClassTogetherForAllHomogeneousSections(CalcModel calculatedModel, Job job);

        List<string> GetListOfHomogeneousSectionListForTheProvidedSectionCode(string sectionCode, List<string> allSectionsList, Job job);

        bool VerifyResultsAndApplyThem(List<SectionTotals> originalListOfSectionBeforeLastClassTogetherCalculation, List<PreviewStudentSection> updatedListOfSections,
            CalcModel calculatedModel, List<PreviewStudentSection> originalHomogeneousList, Job job);

        void FillSectionsToTheOriginalValuesWithTheLeftOverStudents(List<PreviewStudentSection> groupedStudentsBySection, List<PreviewStudentSection> studentsLeftList,
            List<SectionTotals> originalListOfSectionBeforeLastClassTogetherCalculation, Job job);

        void TryToMoveLastClassTogetherGroupsIntoExistingSectionsUpToTotalStudentsInSections(List<PreviewStudentSection> groupedStudentsBySection,
            List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether, List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation,
            List<PreviewStudentSection> homogeneousListToUpdate, Job job);

        void BiggestSectionIsEqualToBiggestClassTogetherGroup(ref LastClassTogetherGroup biggestClassTogetherGroup, List<PreviewStudentSection> groupedStudentsBySection,
            ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether, ref List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation,
            ref bool done);

        void BiggestSectionIsGreaterThanTheBiggestClassTogetherGroup(ref LastClassTogetherGroup biggestClassTogetherGroup, List<PreviewStudentSection> groupedStudentsBySection,
            ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether, ref bool done,
            ref List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation);

        void BiggestSectionIsLessThanTheBiggestClassTogetherGroup(ref LastClassTogetherGroup biggestClassTogetherGroup,
            List<PreviewStudentSection> groupedStudentsBySection, ref SectionTotals sectionWithSeatsAvailableToPutGroupStudentsInto, ref List<LastClassTogetherGroup> listOfGroupsThatTookLastClassTogether,
            ref List<SectionTotals> listOfSectionBeforeLastClassTogetherCalculation, List<PreviewStudentSection> homogeneousListToUpdate, ref bool done);

        List<LastClassTogetherGroup> GetListOfStudentsThatTookClassesTogetherOrderByHighest(List<PreviewStudentSection> listToUpdate);
    }
}
