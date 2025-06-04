using System;

namespace Infrastructure.Utilities
{
    public class BaseConfiguration
    {
        protected DateTime GetCutOffStartDateTime(int daysBeforeLiveJobRuns) { return DateTime.Today.AddDays(daysBeforeLiveJobRuns - 1); }

        protected DateTime GetCutOffEndDateTime(int daysBeforeLiveJobRuns)
        {
            return DateTime.Today.AddDays(daysBeforeLiveJobRuns).Subtract(new TimeSpan(0, 0, 0, 1, 0));
        }
    }
}
