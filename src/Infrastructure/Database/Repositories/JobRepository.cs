using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Infrastructure.Database.Context;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories
{
    public class JobRepository : IJobRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public JobRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void InsertJob(Job job)
        {
            using var context = _dbContextFactory.CreateDbContext();    
            context.Jobs.Add(job);
            context.SaveChanges();
        }

        public void UpdateJob(Job job)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.Entry(job).State = EntityState.Modified;
            context.SaveChanges();
        }

        public List<Job> GetJobListWithStatus()
        {
            using var context = _dbContextFactory.CreateDbContext();

            return context.Jobs.Include(j => j.Status).ToList();
        }
    }
}
