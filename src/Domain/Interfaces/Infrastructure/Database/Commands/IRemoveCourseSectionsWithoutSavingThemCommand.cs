using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IRemoveCourseSectionsWithoutSavingThemCommand
    {
        List<CourseSection> ExecuteCommand(List<CourseSection> recordsToUpdate);
    }
}
