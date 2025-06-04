using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Queries.OneStudentPerSection
{
    public interface IGetActiveOneStudentPerSectionCourseSectionsToCreateQuery
    {
        List<CourseSection> ExecuteQuery(QueryInputParams parameters);
    }
}
