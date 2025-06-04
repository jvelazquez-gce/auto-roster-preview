using Domain.Entities;
using Domain.Models.Helper;
using System;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface ICalculator
    {
        void CalculateSectionsNeeded(PreLoadStudentSection firstRecord, List<PreLoadStudentSection> studentSectionList, CalcModel calculatedModel);

        void UpdateSectionStatusToSuccessAndAddSectionToFinalModel(CalcModel calculatedModel, PreviewStudentSection studentSection);

        void AddAllStudentsToModelInOneSection(CalcModel calculatedModel, List<PreLoadStudentSection> distinctGroupNumberList,
            PreLoadStudentSection firstRecord, string mainGroupNameSuffix, Job job);

        void BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(CalcModel calculatedModel, PreLoadStudentSection firtRecord,
            List<PreLoadStudentSection> distinctSuperSectionList, string groupNameSuffix, ref int counter, Job job);

        List<SectionTotals> GetDistinctSectionListOrderByHighestStudentsInSection(List<PreviewStudentSection> listOfSections);

        List<CourseStartDate> GetCourseStartDateListToProcess(List<PreLoadStudentSection> initialStudentRawData);

        int GetMaxNumberOfStudentsPerSection(int targetStudentCount, Guid? groupNumber, int? groupTargetStudentCount);
    }
}
