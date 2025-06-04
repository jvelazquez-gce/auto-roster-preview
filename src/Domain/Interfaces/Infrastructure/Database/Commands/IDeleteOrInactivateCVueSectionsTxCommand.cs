using Domain.Entities;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IDeleteOrInactivateCVueSectionsTxCommand
    {
        List<CourseSection> Execute(QueryInputParams queryInputParams);
    }
}
