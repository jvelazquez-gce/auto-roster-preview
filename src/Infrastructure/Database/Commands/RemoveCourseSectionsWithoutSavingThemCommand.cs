using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;

namespace Infrastructure.Database.Commands
{
    public class RemoveCourseSectionsWithoutSavingThemCommand : IRemoveCourseSectionsWithoutSavingThemCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public RemoveCourseSectionsWithoutSavingThemCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<CourseSection> ExecuteCommand(List<CourseSection> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(r => context.CourseSections.Remove(r));

            return recordsToUpdate;
        }
    }
}
