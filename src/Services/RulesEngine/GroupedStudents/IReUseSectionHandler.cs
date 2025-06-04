using System.Collections.Generic;
using Domain.Entities;

namespace Services.RulesEngine.GroupedStudents
{
    public interface IReUseSectionHandler
    {
        void FindSectionsToReUseIfPossibleAndAssignStudents();

        void MakeChanges(ClassGroupStudentSection firstCohortMatch, List<ClassGroupStudentSection> cohortMatchList, List<PreLoadStudentSection> tempGroupList);
    }
}
