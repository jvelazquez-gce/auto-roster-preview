using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.RulesEngine.GroupedStudents
{
    public interface IGroupOrganizer
    {
        List<PreLoadStudentSection> GetListWithUpdateGroupDetails(List<PreLoadStudentSection> preLoadStudentSections, Parameters p, List<Rule> rules);
    }
}
