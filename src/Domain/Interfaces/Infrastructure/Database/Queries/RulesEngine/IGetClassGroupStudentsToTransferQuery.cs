using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Queries.RulesEngine
{
    public interface IGetClassGroupStudentsToTransferQuery
    {
        List<ClassGroupStudentSection> ExecuteQuery(QueryInputParams parameters);
    }
}
