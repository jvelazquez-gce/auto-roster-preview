using System.Collections.Generic;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands.RulesEngine;

namespace Infrastructure.Database.Commands.RulesEngine
{
    public class RemoveClassGroupStudentSectionCommand : IRemoveClassGroupStudentSectionCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public RemoveClassGroupStudentSectionCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<ClassGroupStudentSection> ExecuteCommand(List<ClassGroupStudentSection> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(r => context.ClassGroupStudentSections.Remove(r));
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}