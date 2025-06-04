using Domain.Exceptions;
using System.Collections.Generic;
using System.Linq;
using Domain.Entities;

namespace Infrastructure.Utilities
{
    public class UtilErrorChecking
    {
        public void CheckConfigurationValues(List<SystemConfiguration> configurationList)
        {
            if (configurationList == null) throw new ConfigurationException("There was an issue attempting to read the configurations from the ARB database.");
            if (configurationList.Count == 0) throw new ConfigurationException("There were no configuration records in the ARB database.");
            if (configurationList.Count > 1) throw new ConfigurationException("There was more than one configuration in the ARB database.");
            SystemConfiguration firstConfig = configurationList.First();

            if (firstConfig.DaysBeforeLiveJobRuns <= 0) throw new ConfigurationException("The flag to run ARB in advance is equal to or less than zero in the ARB database.");
            if (firstConfig.CohortSectionMinSize <= 0) throw new ConfigurationException("The min inclusive section size is equal to or less than zero in the ARB database.");
            if (string.IsNullOrEmpty(firstConfig.DefaultInstructorEmail) || string.IsNullOrWhiteSpace(firstConfig.DefaultInstructorEmail)) 
                throw new ConfigurationException("The default instructor email address is empty in the ARB database.");
            if (firstConfig.MaxTransferRecordsPerBatch <= 0) throw new ConfigurationException("The number of records per batch to transfer is equal to or less than zero in the ARB database.");
            if (firstConfig.CMCMaxSectionsPerBatch <= 0) throw new ConfigurationException("The number of sections per batch is equal to or less than zero in the ARB database.");
            if (firstConfig.MaxNumberOfForecastSections <= 0) throw new ConfigurationException("The max number of forecast sections is equal to or less than zero in the ARB database.");
        }
    }
}
