using Domain.Constants;

namespace Domain.Interfaces.Infrastructure.Utilities
{
    public interface IFeatureToggle
    {
        bool IsFeatureEnabled(Feature featureName);
    }
}
