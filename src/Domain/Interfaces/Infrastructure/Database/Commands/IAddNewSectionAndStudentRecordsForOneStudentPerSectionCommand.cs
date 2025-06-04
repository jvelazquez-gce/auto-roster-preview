using Domain.Models.Helper;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IAddNewSectionAndStudentRecordsForOneStudentPerSectionCommand
    {
        OneStudentPerSectionPair ExecuteCommand(OneStudentPerSectionPair pairRecord);
    }
}
