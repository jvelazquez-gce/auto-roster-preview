using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IUpdateCourseSectionCommand
    {
        void Execute(List<CourseSection> courseSectionsToUpdate);
    }
}
