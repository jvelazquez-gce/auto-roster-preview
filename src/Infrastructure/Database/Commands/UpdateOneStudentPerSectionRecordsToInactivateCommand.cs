using Domain.Models.Helper;
using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Commands
{
    public class UpdateOneStudentPerSectionRecordsToInactivateCommand : IUpdateOneStudentPerSectionRecordsToInactivateCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public UpdateOneStudentPerSectionRecordsToInactivateCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<OneStudentPerSectionPair> ExecuteCommand(List<OneStudentPerSectionPair> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            foreach (var pairRecord in recordsToUpdate)
            {
                pairRecord.CourseSections.ForEach(c => context.Entry(c).State = EntityState.Modified);
                pairRecord.OneStudentPerSectionList.ForEach(s => context.Entry(s).State = EntityState.Modified);
            }
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}
