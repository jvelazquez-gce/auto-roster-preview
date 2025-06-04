using Domain.Entities;
using Domain.Models.Helper;
using System;
using System.Collections.Generic;

namespace Services.Calculators
{
    public interface IBalancer
    {
        public void CheckForRecordsWithDataErrorsAndRemoveThem(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            Job job,
            int errorCheckType);

        public CalcModel ExcecuteLogicForUniqueCourseAndStartDate(
            int courseId,
            DateTime startDate,
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            Job job,
            Parameters p = null);

        public CalcModel UpdateCalcModelWithOneStudentPerSectionRecords(
            CalcModel calcModel, 
            OneStudentPerSectionResults oneStudentPerSectionResults, 
            Job job);

        public CalcModel GetSectionsThatNeedToBeCreated(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            CalcModel calcModel, 
            Job job, 
            Parameters p);

        public CalcModel CreateNonExclusiveCohortSections(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, 
            Job job);

        public void CombineNonExclusiveCohortSectionsIfPossibleToCreateTheLeastAmountOfSections(
            CalcModel nonExclusiveCohortsCalcModel, 
            Job job);

        public CalcModel CreateExclusiveSections(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            Job job);

        public void FillNonExclusiveCohortSectionsWithNoGroupStudentsAndCreateNoGroupSectionsIfNeeded(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            CalcModel calculatedModel, 
            Job job);

        public void SaveToArbDb(CalcModel calculatedModel, Parameters p, Job job);
    }
}
