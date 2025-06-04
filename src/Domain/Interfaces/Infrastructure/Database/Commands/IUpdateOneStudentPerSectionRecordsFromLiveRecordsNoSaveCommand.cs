using Domain.Models.Helper;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IUpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand
    {
        OneStudentPerSectionResults ExecuteCommand(OneStudentPerSectionResults recordsToUpdate);
    }
}
