using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Queries.RulesEngine;
using Domain.Models.Helper;
using Infrastructure.Database.Context;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Database.Queries.RulesEngine
{
    public class GetActiveRulesByCourseIdQuery : IGetActiveRulesByCourseIdQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public GetActiveRulesByCourseIdQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public List<Rule> ExecuteQuery(QueryInputParams parameters)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.Rules
                .Where(w => w.AdCourseID == parameters.AdCourseID)
                .Where(w => w.IsActive)
                .OrderBy(o => o.Priority)
                .ToList();
        }
    }
}
