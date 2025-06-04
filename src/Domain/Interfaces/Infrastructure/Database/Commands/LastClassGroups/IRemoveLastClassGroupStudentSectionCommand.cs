using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups
{
    public interface IRemoveLastClassGroupStudentSectionCommand
    {
        List<LastClassGroupStudentSection> ExecuteCommand(List<LastClassGroupStudentSection> recordsToUpdate);
    }
}
