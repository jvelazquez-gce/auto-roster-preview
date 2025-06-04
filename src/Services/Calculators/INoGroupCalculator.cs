using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface INoGroupCalculator
    {
        void GetCalculatedSectionsModel(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, CalcModel calculatedModel, Job job);

        CalcModel AddStudentsToNonExclusiveCohortSections(CalcModel calculatedModel, List<PreLoadStudentSection> distinctNoGroupSectionStudentList, Job job);

        CalcModel AddStudentsToNotExlusiveCohortSections(CalcModel calculatedModel, List<PreLoadStudentSection> distinctNoGroupSectionStudentList,
            List<SectionTotals> uniqueBiggestToSmallestSections, Job job);

        void DetermineHowManyNonGroupStudentsToMoveIntoOneSectionAndMoveThem(CalcModel calculatedModel, List<PreLoadStudentSection> distinctNoGroupSectionStudentList,
            SectionTotals section, int seatsAvailableInSection, Job job);

        void AddNonGroupStudentsToExistingSection(CalcModel calculatedModel, SectionTotals section,
            int numberOfNonGroupStudentsToTake, List<PreLoadStudentSection> distinctNoGroupSectionStudentList, Job job);

        CalcModel CreateNewSections(CalcModel calculatedModel, PreLoadStudentSection firtRecord,
            List<PreLoadStudentSection> noGroupSectionStudentList, Job job);

        List<PreviewStudentSection> NonCohortPreviewStudentRecords(List<PreviewStudentSection> previewRecords);

        List<PreLoadStudentSection> NonCohortPreLoadStudentRecords(List<PreLoadStudentSection> preLoadRecords);
    }
}
