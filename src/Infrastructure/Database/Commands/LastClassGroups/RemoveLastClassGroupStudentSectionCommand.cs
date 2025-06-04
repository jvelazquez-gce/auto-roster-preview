using System.Collections.Generic;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;

namespace Infrastructure.Database.Commands.LastClassGroups
{
    public class RemoveLastClassGroupStudentSectionCommand : IRemoveLastClassGroupStudentSectionCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public RemoveLastClassGroupStudentSectionCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<LastClassGroupStudentSection> ExecuteCommand(List<LastClassGroupStudentSection> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(r => context.LastClassGroupStudentSections.Remove(r));
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}
