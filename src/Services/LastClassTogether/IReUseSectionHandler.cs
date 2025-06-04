using System.Collections.Generic;
using Domain.Entities;

namespace Services.LastClassTogether
{
    public interface IReUseSectionHandler
    {
        void FindSectionsToReUseIfPossibleAndAssignStudents();

        void MakeChanges(LastClassGroupStudentSection firstCohortMatch, List<LastClassGroupStudentSection> cohortMatchList, List<PreLoadStudentSection> tempGroupList);
    }
}
