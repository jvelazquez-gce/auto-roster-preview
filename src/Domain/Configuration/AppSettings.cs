using System;

namespace Domain.Configuration
{
    public class AppSettings
    {
        //get { return Convert.ToInt32(ConfigurationManager.AppSettings["MaxNumerOfLogsToDisplay"]); }
        public int MAX_NUMBER_OF_LOGS_TO_DISPLAY { get; set; }

        // Use for unit test only. In prod the value is road from the db.
        //get { return Convert.ToInt32(ConfigurationManager.AppSettings["DaysBeforeLiveJobRuns"]); }
        public int DAYS_BEFORE_LIVE_JOB_RUNS { get; set; }

        //get { return ConfigurationManager.AppSettings["Email.Smtp"]; }
        public string EmailSmptServer { get; set; }

        //get { return ConfigurationManager.AppSettings["Email.From.Address"]; }
        public string EmailFrom { get; set; }

        //get { return ConfigurationManager.AppSettings["Email.To.Address"]; }
        public string EmailTo { get; set; }
        public string EmailBodyErrorMsg { get; set; }

        //get { return ConfigurationManager.AppSettings["Email.Subject"]; }
        public string EmailSubject { get; set; }

        //get { return Convert.ToInt32(ConfigurationManager.AppSettings["MaxCourseStartDateListToLogAtATime"]); }
        public int MAX_COURSE_START_DATE_LIST_TO_LOG_AT_A_TIME { get; set; } = 200;

        // Use for unit test only. In prod the value is road from the db.
        //get { return DateTime.Today.AddDays(GeneralConfiguration.DAYS_BEFORE_LIVE_JOB_RUNS - 1); }
        public DateTime CUT_OFF_START_DATE_TIME { get; set; }

        // Use for unit test only. In prod the value is road from the db.
        //get { return DateTime.Today.AddDays(GeneralConfiguration.DAYS_BEFORE_LIVE_JOB_RUNS).Subtract(new TimeSpan(0, 0, 0, 1, 0)); }
        public DateTime CUT_OFF_END_DATE_TIME { get; set; }

        public string DbConnectionString { get; set; }
    }
}
