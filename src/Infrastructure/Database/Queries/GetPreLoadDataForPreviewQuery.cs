using Domain.Constants;
using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Infrastructure.Database.Queries;
using Infrastructure.Database.Context;
using Domain.Entities;

namespace Infrastructure.Database.Queries
{
    public class GetPreLoadDataForPreviewQuery : IGetPreLoadDataForPreviewQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetPreLoadDataForPreviewQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<PreLoadStudentSection> ExecuteQuery(QueryInputParams queryInputParams)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.PreLoadStudentSections
                .Where(r => r.StartDate > queryInputParams.CutOffEndDateTime)
                .Where(r => r.StatusID != SectionStatus.INACTIVE)
                .ToList();
        }
    }
}
