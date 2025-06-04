using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Interfaces.Infrastructure.Database.Repositories
{
    public interface IConfigurationRepository
    {
        void Add(SystemConfiguration systemConfiguration);

        List<SystemConfiguration> GetSystemConfigurations();

        void UpdateSystemConfiguration(SystemConfiguration systemConfiguration);

        void DeleteSystemConfiguration(int id);

        List<Entities.Other.Configuration> GetConfigurations();
    }
}
