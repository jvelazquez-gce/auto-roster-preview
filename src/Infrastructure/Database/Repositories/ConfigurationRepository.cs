using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Infrastructure.Database.Context;
using Domain.Entities;
using Domain.Entities.Other;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IDbContextFactory _dbContextFactory;

        public ConfigurationRepository(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Add(SystemConfiguration configuration)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.SystemConfigurations.Add(configuration);
            context.SaveChanges();
        }

        public List<SystemConfiguration> GetSystemConfigurations()
        {
            using var context = _dbContextFactory.CreateDbContext();
            {
                context.Database.SetCommandTimeout(5000);
                var result = context.SystemConfigurations.ToListAsync().Result;
                return result;
            }
        }

        public List<Configuration> GetConfigurations()
        {
            using var context = _dbContextFactory.CreateDbContext();
            {
                context.Database.SetCommandTimeout(5000);
                var result = context.Configurations.ToListAsync().Result;
                return result;
            }
        }

        public void UpdateSystemConfiguration(SystemConfiguration configuration)
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.Entry(configuration).State = EntityState.Modified;
            context.SaveChanges();
        }

        public void DeleteSystemConfiguration(int id)
        {
            using var context = _dbContextFactory.CreateDbContext();
            SystemConfiguration configuration = context.SystemConfigurations.Find(id);
            context.SystemConfigurations.Remove(configuration);
            context.SaveChanges();
        }
    }
}
