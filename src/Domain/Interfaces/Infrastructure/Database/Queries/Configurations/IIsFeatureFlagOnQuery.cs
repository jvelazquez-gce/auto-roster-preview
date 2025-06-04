using Domain.Models.Helper;

namespace Domain.Interfaces.Infrastructure.Database.Queries.Configurations
{
    public interface IIsFeatureFlagOnQuery
    {
        bool ExecuteQuery(QueryInputParams queryInputParams);
    }
}
