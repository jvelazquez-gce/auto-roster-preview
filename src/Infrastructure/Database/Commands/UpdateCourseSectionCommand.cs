using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Infrastructure.Database.Commands
{
    public class UpdateCourseSectionCommand : IUpdateCourseSectionCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public UpdateCourseSectionCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute(List<CourseSection> courseSectionsToUpdate)
        {
            if (courseSectionsToUpdate.Count == 0) return;

            using var context = _dbContextFactory.CreateDbContext();
            courseSectionsToUpdate.ForEach(s => context.Entry(s).State = EntityState.Modified);
            context.SaveChanges();
        }
    }
}
