using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Queries.LastClassGroupSection
{
    public interface IGetActiveLastClassGroupCourseSectionsToCreateQuery
    {
        List<CourseSection> ExecuteQuery(QueryInputParams parameters);
    }
}
