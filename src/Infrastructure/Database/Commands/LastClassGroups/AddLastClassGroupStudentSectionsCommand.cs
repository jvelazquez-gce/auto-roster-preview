using System.Collections.Generic;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;

namespace Infrastructure.Database.Commands.LastClassGroups
{
    public class AddLastClassGroupStudentSectionsCommand : IAddLastClassGroupStudentSectionsCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AddLastClassGroupStudentSectionsCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<LastClassGroupStudentSection> ExecuteCommand(List<LastClassGroupStudentSection> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(c => context.LastClassGroupStudentSections.Add(c));
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}
