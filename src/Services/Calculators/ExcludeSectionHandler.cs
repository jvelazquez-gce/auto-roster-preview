using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Models.Helper;
using Domain.Entities;

namespace Services.Calculators
{
    public class ExcludeSectionHandler
    {
        public CalcModel CheckIfThereAreSectionsThatCanBeExcludedAndExcludeThem(
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, 
            CalcModel calcModel, 
            Job job)
        {
            List<PreLoadStudentSection> studentSectionRecordsToExclude = GetStudentSectionRecordsToExclude(sectionsWithSameCourseAndStartDate);

            if (studentSectionRecordsToExclude.Count == 0) return calcModel;

            studentSectionRecordsToExclude = AddMissingRecordsToTheStudentSectionRecordsToExclude(studentSectionRecordsToExclude,
                sectionsWithSameCourseAndStartDate);

            UpdateStatusOfRecordsToExcludeAndExcludeThemByUpdatingListsAndCalcModel(studentSectionRecordsToExclude,
                sectionsWithSameCourseAndStartDate, calcModel);

            return calcModel;
        }

        public List<PreLoadStudentSection> GetStudentSectionRecordsToExclude(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate)
        {
            return sectionsWithSameCourseAndStartDate
                .Where(r => r.GroupTypeKey == Group.EXCLUSIVE_COHORT)
                .Where(r => !string.IsNullOrWhiteSpace(r.ProgramLetterInitial))
                .Where(r => !r.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER))
                .Where(r => r.SectionCode.ToLower().Equals(r.ProgramLetterInitial.ToLower()))
                .ToList();
        }

        public List<PreLoadStudentSection> AddMissingRecordsToTheStudentSectionRecordsToExclude(
            List<PreLoadStudentSection>  studentSectionRecordsToExclude, 
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate)
        {
            List<string> DistinctSectionCodeNameList = studentSectionRecordsToExclude.Select(s => s.SectionCode).Distinct().ToList();

            List<PreLoadStudentSection> exclusiveStudentSectionsThatDontMatchButShuldBeExcluded =
                sectionsWithSameCourseAndStartDate.Where(w => !studentSectionRecordsToExclude.Contains(w)
                    && DistinctSectionCodeNameList.Contains(w.SectionCode)).ToList();

            studentSectionRecordsToExclude.AddRange(exclusiveStudentSectionsThatDontMatchButShuldBeExcluded);

            return studentSectionRecordsToExclude;
        }

        public void UpdateStatusOfRecordsToExcludeAndExcludeThemByUpdatingListsAndCalcModel(
            List<PreLoadStudentSection> studentSectionRecordsToExclude, 
            List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, 
            CalcModel calcModel)
        {
            foreach (var studentSection in studentSectionRecordsToExclude)
            {
                sectionsWithSameCourseAndStartDate.Remove(studentSection);
                studentSection.StatusID = StudentSectionStatus.EXCLUDED_FROM_ARB_RULE;
                calcModel.SectionsToExclude.Add(studentSection);
            }
        }
    }
}
