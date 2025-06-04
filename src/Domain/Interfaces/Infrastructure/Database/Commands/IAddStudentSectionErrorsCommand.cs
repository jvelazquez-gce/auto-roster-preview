using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IAddStudentSectionErrorsCommand
    {
        List<StudentSectionError> ExecuteCommand(List<StudentSectionError> recordsToUpdate);
    }
}
