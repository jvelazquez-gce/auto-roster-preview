using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Queries
{
    public interface IGetPreLoadDataForUpdateQuery
    {
        List<PreLoadStudentSection> ExecuteQuery(QueryInputParams queryInputParams);
    }
}
