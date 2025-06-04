using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.OneStudentPerSection;

namespace Infrastructure.Database.Queries.OneStudentPerSection
{
    public class GetPreviewOneStudentPerSectionStudentsToTransferQuery : IGetPreviewOneStudentPerSectionStudentsToTransferQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetPreviewOneStudentPerSectionStudentsToTransferQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<OneStudentPerSectionRecord> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.OneStudentPerSectionRecords
                .Where(w => w.StartDate == parameters.StartDate)
                .Where(w => w.AdCourseID == parameters.AdCourseID)
                .Where(w => w.StatusID == StudentSectionStatus.READY_TO_BE_TRANSFER_IN_CVUE)
                .ToList();
        }
    }
}
