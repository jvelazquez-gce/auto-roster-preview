using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands.RulesEngine
{
    public interface IRemoveClassGroupStudentSectionCommand
    {
        List<ClassGroupStudentSection> ExecuteCommand(List<ClassGroupStudentSection> recordsToUpdate);
    }
}
