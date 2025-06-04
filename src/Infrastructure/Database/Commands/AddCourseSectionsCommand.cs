using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;

namespace Infrastructure.Database.Commands
{
    public class AddCourseSectionsCommand : IAddCourseSectionsCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public AddCourseSectionsCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<CourseSection> ExecuteCommand(List<CourseSection> recordsToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            recordsToUpdate.ForEach(c => context.CourseSections.Add(c));
            context.SaveChanges();

            return recordsToUpdate;
        }
    }
}
