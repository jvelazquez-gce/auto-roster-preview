using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Repositories
{
    public interface IJobRepository
    {
        void InsertJob(Job job);
        void UpdateJob(Job job);
        List<Job> GetJobListWithStatus();
    }
}
