using Domain.Entities;
using Domain.Models.Helper;
using System;

namespace Infrastructure.Utilities
{
    public class MyLogger
    {
        public static Log GetDefaultLog(Guid processId, string msg, int jobId, string level)
        {
            var log = new Log()
            {
                JobID = jobId,
                ProcessID = processId,
                CourseID = 0,
                CourseStartDate = new DateTime(1800, 1, 1),
                Date = DateTime.Now,
                Thread = "none",
                Level = level,
                Logger = "Preview",
                Message = msg,
                Exception = ""
            };
            return log;
        }

        public static void CreateLog(Parameters p, string msg, int courseId, DateTime startDate, string logLevel, int jobId, Guid processId)
        {
            var log = GetDefaultLog(processId, msg, jobId, logLevel);
            log.CourseID = courseId;
            log.CourseStartDate = startDate;
            p.LogCommand.Execute(log);
        }
    }
}
