using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.OneStudentPerSection;

namespace Infrastructure.Database.Queries.OneStudentPerSection
{
    public class GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery : IGetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        // TODO: may want to make more explicit by getting where status id  = SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE = 59 or 60
        public List<CourseSection> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();
            //_context.Database.CommandTimeout = GeneralConfiguration.DB_QUERY_TIMEOUT_TIME_IN_SECONDS;
            return context.CourseSections
                .Where(r => r.StartDate == parameters.StartDate)
                .Where(r => r.AdCourseID == parameters.AdCourseID)
                .Where(r => r.MaximumNumberOfStudents == 1)
                .Where(r => !r.SectionCode.Equals(GeneralStatus.ONLINE_SUPER_SECTION_IDENTIFIER))
                .Where(r => r.GroupCategory == ARBGroupCategory.ONE_STUDENT_PER_SECTION)
                .Where(r => (!r.HasSectionBeenCreatedOrUpdatedInCampusVue && r.StatusID == SectionStatus.READY_TO_CREATE_SECTION_IN_CVUE) 
                              ||  
                            (r.HasSectionBeenCreatedOrUpdatedInCampusVue && r.StatusID == SectionStatus.SUCCESSFULLY_CREATED_SECTION_IN_CVUE))
                .ToList();
        }

    }
}
