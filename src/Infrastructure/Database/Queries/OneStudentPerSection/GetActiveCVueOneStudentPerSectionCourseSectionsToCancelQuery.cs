using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.OneStudentPerSection;

namespace Infrastructure.Database.Queries.OneStudentPerSection
{
    public class GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery : IGetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<CourseSection> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.CourseSections
                .Where(r => r.StartDate == parameters.StartDate)
                .Where(r => r.AdCourseID == parameters.AdCourseID)
                .Where(r => r.GroupCategory == ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                .Where(r => r.AdClassSchedId.HasValue)
                .Where(r => r.AdClassSchedId != 0)
                .Where(r => r.AdClassSchedId != null)
                .Where(r => !r.HasSectionBeenCreatedOrUpdatedInCampusVue)
                .Where(r => r.StatusID == SectionStatus.READY_TO_CANCEL_SECTION_IN_CVUE)
                .ToList();
        }
    }
}
