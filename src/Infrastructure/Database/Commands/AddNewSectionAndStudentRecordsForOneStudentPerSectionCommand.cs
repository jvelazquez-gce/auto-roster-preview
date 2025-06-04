using Domain.Models.Helper;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;

namespace Infrastructure.Database.Commands
{
    public class AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand : IAddNewSectionAndStudentRecordsForOneStudentPerSectionCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public OneStudentPerSectionPair ExecuteCommand(OneStudentPerSectionPair pairRecord)
        {
            using var context = _dbContextFactory.CreateDbContext();
            pairRecord.CourseSections.ForEach(c => context.CourseSections.Add(c));
            pairRecord.OneStudentPerSectionList.ForEach(s => context.OneStudentPerSectionRecords.Add(s));
            context.SaveChanges();

            return pairRecord;
        }
    }
}
