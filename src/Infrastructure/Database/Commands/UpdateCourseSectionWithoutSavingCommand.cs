using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Commands
{
    public class UpdateCourseSectionWithoutSavingCommand : IUpdateCourseSectionWithoutSavingCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public UpdateCourseSectionWithoutSavingCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public CourseSection ExecuteCommand(CourseSection recordToUpdate)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.Entry(recordToUpdate).State = EntityState.Modified;
            return recordToUpdate;
        }
    }
}
