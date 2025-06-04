using System;
using System.Diagnostics;
using System.Net.Mail;

namespace Infrastructure.Utilities
{
    public class Email
    {
        public static bool SendMail(string body, string subject, string from, string to, string smtpServer)
        {
            bool success = false;

            MailMessage myMail = new MailMessage();
            myMail.From = new MailAddress(from);
            myMail.To.Add(to);
            myMail.Subject = subject;
            myMail.IsBodyHtml = true;
            myMail.Body = body;

            SmtpClient mySMTP = new SmtpClient(smtpServer);
            try
            {
                mySMTP.Send(myMail);
                success = true;
            }
            catch (SmtpException smtpEx)
            {
                string logStr = "SMTPError";
                if (!EventLog.SourceExists(logStr))
                {
                    EventLog.CreateEventSource(logStr, logStr);
                }
                var logEntry = new EventLog();
                logEntry.Source = logStr;
                logEntry.WriteEntry(smtpEx.Message.Replace("'", "\'"), EventLogEntryType.Error);
                success = false;
            }
            catch (Exception generalEx)
            {
                string logStr = "AspNetError";
                if (!EventLog.SourceExists(logStr))
                {
                    EventLog.CreateEventSource(logStr, logStr);
                }
                var logEntry = new EventLog();
                logEntry.Source = logStr;
                logEntry.WriteEntry(generalEx.Message.Replace("'", "\'"), EventLogEntryType.Error);
                success = false;
            }

            return success;
        }
    }
}
