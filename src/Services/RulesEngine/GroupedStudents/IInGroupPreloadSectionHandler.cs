using Domain.Entities;
using System.Collections.Generic;

namespace Services.RulesEngine.GroupedStudents
{
    public interface IInGroupPreloadSectionHandler
    {
        List<PreLoadStudentSection> FindAndMarkRecordsAsInRuleGroupRemoveNotInRuleGroupFromDbAndStudentCleanUp();
    }
}
