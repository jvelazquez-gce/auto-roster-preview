using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IAddCourseSectionsCommand
    {
        List<CourseSection> ExecuteCommand(List<CourseSection> recordsToUpdate);
    }
}
