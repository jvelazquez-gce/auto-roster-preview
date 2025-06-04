using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Entities;
using Domain.Entities.Other;

namespace Infrastructure.Utilities
{
    public class ConfigInstance : IConfigInstance
    {
        private SystemConfiguration _sysConfiguration;
        private List<Configuration> _configurations;

        public ConfigInstance(IConfigurationRepository configurationRepository)
        {
            var systemConfigurations = configurationRepository.GetSystemConfigurations();
            new UtilErrorChecking().CheckConfigurationValues(systemConfigurations);
            _sysConfiguration = systemConfigurations.First();
            _configurations = configurationRepository.GetConfigurations();
        }

        public bool RunARBProcess() { return _sysConfiguration.RunARBProcess; }

        public int DaysBeforeLiveJobRuns()
        {
            if (_sysConfiguration.DaysBeforeLiveJobRuns > 0)
                return _sysConfiguration.DaysBeforeLiveJobRuns;

            throw new ConfigurationException("There was an issue with the value for DaysBeforeLiveJobRuns. Please check the value in the ARB database.");
        }

        public DateTime GetCutOffStartDateTime() => GetCutOffStartDateTime(DaysBeforeLiveJobRuns());

        private DateTime GetCutOffStartDateTime(int daysBeforeLiveJobRuns) => DateTime.Today.AddDays(daysBeforeLiveJobRuns - 1);

        public DateTime GetCutOffEndDateTime() => GetCutOffEndDateTime(DaysBeforeLiveJobRuns());

        private DateTime GetCutOffEndDateTime(int daysBeforeLiveJobRuns)
        {
            return DateTime.Today.AddDays(daysBeforeLiveJobRuns).Subtract(new TimeSpan(0, 0, 0, 1, 0));
        }

        public int GetCohortMinSize() { return _sysConfiguration.CohortSectionMinSize; }
        public string GetDefaultInstructorEmail() { return _sysConfiguration.DefaultInstructorEmail; }
        public int GetDefaultInstructorID() { return _sysConfiguration.DefaultInstructorID; }
        public int MaxTransferRecordsPerBatch() { return _sysConfiguration.MaxTransferRecordsPerBatch; }
        public int GetCMCMaxSectionsPerBatch() { return _sysConfiguration.CMCMaxSectionsPerBatch; }
        public int GetNumberOfDaysWhenToStartDeletingCourses() { return _sysConfiguration.NumberOfDaysWhenToStartDeletingCourses; }
        public int MaxNumberOfForecastSections() { return _sysConfiguration.MaxNumberOfForecastSections; }
        public List<Configuration> GetConfigurationList() { return _configurations;  }
    }
}
