using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Entities;
using Domain.Entities.Other;

namespace Infrastructure.Utilities
{
    public class Config : BaseConfiguration
    {
        private SystemConfiguration _systemConfiguration;
        private List<Configuration> _configurations;

        public Config(IConfigurationRepository configurationRepository)
        {
            var systemConfigurations = configurationRepository.GetSystemConfigurations();
            new UtilErrorChecking().CheckConfigurationValues(systemConfigurations);
            _systemConfiguration = systemConfigurations.First();
            _configurations = configurationRepository.GetConfigurations();
        }

        public int DaysBeforeLiveJobRuns()
        {
            if (_systemConfiguration.DaysBeforeLiveJobRuns > 0)
                return _systemConfiguration.DaysBeforeLiveJobRuns;

            throw new ConfigurationException("There was an issue with the value for DaysBeforeLiveJobRuns. Please check the value in the ARB database.");
        }

        public DateTime GetCutOffStartDateTime() { return GetCutOffStartDateTime(DaysBeforeLiveJobRuns()); }

        public DateTime GetCutOffEndDateTime()
        {
            return GetCutOffEndDateTime(DaysBeforeLiveJobRuns());
        }

        public List<Configuration> GetConfigurationList() { return _configurations; }
    }
}
