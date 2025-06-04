using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Interfaces.Infrastructure.Utilities;
using Domain.Entities.Other;

namespace Services.Utilities
{
    public class FeatureToggle : IFeatureToggle
    {
        private readonly Dictionary<Feature, bool> _dictFeatures = new Dictionary<Feature, bool>();
        private readonly IConfigurationRepository _configurationRepository;
        public FeatureToggle(IConfigurationRepository configurationRepository) 
        {
            _configurationRepository = configurationRepository;
        }

        public bool IsFeatureEnabled(Feature featureName)
        {
            Configuration record = null;
            try
            {
                var wasFeatureFoundInDictionary = _dictFeatures.TryGetValue(featureName, out var isFlagOn);

                if (wasFeatureFoundInDictionary)
                    return isFlagOn;

                record = _configurationRepository.GetConfigurations().FirstOrDefault(x => x.Name.Equals(featureName.ToString()));

                if (record == null)
                    return false;

                isFlagOn = Convert.ToBoolean(record.Value);

                _dictFeatures.Add(featureName, isFlagOn);

                return isFlagOn;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
