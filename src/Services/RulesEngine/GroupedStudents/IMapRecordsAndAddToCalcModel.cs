using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.RulesEngine.GroupedStudents
{
    public interface IMapRecordsAndAddToCalcModel
    {
        void UpdateSectionStatusToSuccessAndAddSectionToFinalModel(CalcModel calculatedModel, ClassGroupStudentSection studentSection);

        void AddAllStudentsToModelInOneSection(CalcModel calculatedModel, List<PreLoadStudentSection> distinctGroupNumberList,
            PreLoadStudentSection firstRecord, string mainGroupNameSuffix);

        void BreakGroupIntoMoreThanOneSectionAndAddStudentsToModel(CalcModel calculatedModel, PreLoadStudentSection firstRecord,
            List<PreLoadStudentSection> distinctSuperSectionList, string groupNameSuffix, ref int counter);
    }
}
