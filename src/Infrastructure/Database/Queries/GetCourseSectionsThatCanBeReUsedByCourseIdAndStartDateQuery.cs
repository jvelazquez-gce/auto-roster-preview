using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Infrastructure.Database.Queries;
using Infrastructure.Database.Context;
using Domain.Entities;

namespace Infrastructure.Database.Queries
{
    public class GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery : IGetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<CourseSection> ExecuteQuery(QueryInputParams queryInputParams)
        {
            using var context = _dbContextFactory.CreateDbContext();
            // Get sections CVue sections with instructors that can be re-used or ARB sections with or without
            // instructors to be re-used.
            return context.CourseSections
                .Where(s => s.AdCourseID == queryInputParams.AdCourseID)
                .Where(s => s.StartDate == queryInputParams.StartDate)
                .Where(s => s.StatusID != SectionStatus.INACTIVE)
                .Where(s => ((s.HasInstructor && s.AdClassSchedId.HasValue && s.AdClassSchedId > 0) || (!s.AdClassSchedId.HasValue || s.AdClassSchedId == 0)))
                .Where(s => s.GroupCategory != ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                .Where(s => s.GroupCategory != ARBGroupCategory.LAST_CLASS_TOGETHER_COHORT)
                .Where(s => !s.HasSectionBeenCreatedOrUpdatedInCampusVue)
                .ToList();
        }
    }
}
