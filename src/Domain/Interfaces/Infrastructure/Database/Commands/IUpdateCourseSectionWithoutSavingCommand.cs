using Domain.Entities;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IUpdateCourseSectionWithoutSavingCommand
    {
        CourseSection ExecuteCommand(CourseSection recordToUpdate);
    }
}
