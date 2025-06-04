using System;

namespace Services.Scheduler
{
    public class SchedulerTimeCheck
    {
        public bool CanTheArbJobRunRightNow(DateTime now)
        {
            // The XRM job runs every hour on the hour and it takes about 10 minutes. Let's not run it 15 before and after it runs.
            var maxMinuteRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 45, 0);
            var minMinuteRange = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, 15, 0);

            if (now > maxMinuteRange) return false;
            if (now < minMinuteRange) return false;

            return true;
        }
    }
}
