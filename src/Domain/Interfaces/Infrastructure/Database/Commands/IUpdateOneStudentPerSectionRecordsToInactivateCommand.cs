using Domain.Models.Helper;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IUpdateOneStudentPerSectionRecordsToInactivateCommand
    {
        List<OneStudentPerSectionPair> ExecuteCommand(List<OneStudentPerSectionPair> recordsToUpdate);
    }
}
