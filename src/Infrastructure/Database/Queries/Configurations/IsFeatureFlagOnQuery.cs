using Domain.Models.Helper;
using System;
using System.Linq;
using Infrastructure.Database.Context;
using Domain.Entities.Other;
using Domain.Interfaces.Infrastructure.Database.Queries.Configurations;

namespace Infrastructure.Database.Queries.Configurations
{
    public class IsFeatureFlagOnQuery : IIsFeatureFlagOnQuery
    {
        private readonly IDbContextFactory _dbContextFactory;

        public IsFeatureFlagOnQuery(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public bool ExecuteQuery(QueryInputParams queryInputParams)
        {
            Configuration record = null;
            try
            {
                using var context = _dbContextFactory.CreateDbContext();
                record = context.Configurations.FirstOrDefault(x => x.Name.Equals(queryInputParams.FeatureName.ToString()));

                if (record == null)
                    return false;

                return Convert.ToBoolean(record.Value);
            }
            catch (Exception)
            {
                string errorMessage = record == null
                    ? "Error reading feature flag!"
                    : string.Format("Error reading feature flag name: {0} with feature flag value: {1} ", record.Name, record.Value);

                return false;
            }
        }
    }
}
