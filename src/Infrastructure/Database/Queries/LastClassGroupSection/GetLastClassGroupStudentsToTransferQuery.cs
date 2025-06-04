using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.LastClassGroupSection;

namespace Infrastructure.Database.Queries.LastClassGroupSection
{
    public class GetLastClassGroupStudentsToTransferQuery : IGetLastClassGroupStudentsToTransferQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetLastClassGroupStudentsToTransferQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<LastClassGroupStudentSection> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.LastClassGroupStudentSections
               .Where(w => w.StartDate == parameters.StartDate)
               .Where(w => w.AdCourseID == parameters.AdCourseID)
               .Where(w => w.StatusID == StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE)
               .ToList();
        }
    }
}
