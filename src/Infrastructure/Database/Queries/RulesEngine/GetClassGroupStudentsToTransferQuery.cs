using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.RulesEngine;

namespace Infrastructure.Database.Queries.RulesEngine
{
    public class GetClassGroupStudentsToTransferQuery : IGetClassGroupStudentsToTransferQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetClassGroupStudentsToTransferQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<ClassGroupStudentSection> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.ClassGroupStudentSections
               .Where(w => w.StartDate == parameters.StartDate)
               .Where(w => w.AdCourseID == parameters.AdCourseID)
               .Where(w => w.StatusID == StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE)
               .ToList();
        }
    }
}
