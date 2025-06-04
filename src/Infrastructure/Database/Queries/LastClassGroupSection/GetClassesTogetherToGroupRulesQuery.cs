using Domain.Models.Helper;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.LastClassGroupSection;

namespace Infrastructure.Database.Queries.LastClassGroupSection
{
    public class GetClassesTogetherToGroupRulesQuery : IGetClassesTogetherToGroupRulesQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetClassesTogetherToGroupRulesQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<ClassesTogetherToGroupRule> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return context.ClassesTogetherToGroupRules
                .ToList();
        }
    }
}
