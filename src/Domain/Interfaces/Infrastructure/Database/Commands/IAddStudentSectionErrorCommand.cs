using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IAddStudentSectionErrorCommand
    {
        List<StudentSectionError> ExecuteCommand(List<StudentSectionError> recordsToUpdate);
    }
}
