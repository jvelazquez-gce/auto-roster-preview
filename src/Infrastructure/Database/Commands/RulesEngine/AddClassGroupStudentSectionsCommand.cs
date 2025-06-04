using System.Collections.Generic;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands.RulesEngine;

namespace Infrastructure.Database.Commands.RulesEngine
{
    public class AddClassGroupStudentSectionsCommand : IAddClassGroupStudentSectionsCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AddClassGroupStudentSectionsCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<ClassGroupStudentSection> ExecuteCommand(List<ClassGroupStudentSection> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(c => context.ClassGroupStudentSections.Add(c));
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}
