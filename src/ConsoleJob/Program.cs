using Domain.Configuration;
using Domain.Constants;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Infrastructure.Utilities;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using Services.Jobs;
using System;
using System.Linq;

namespace ConsoleJob
{
    public class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new AppSettings();
            ILogCommand _logCommand = null;
            TelemetryClient _telemetryClient;
            IJobService _jobService = null;
            IJobRepository _jobRepository = null;
            try
            {
                var dependencyInjection = new DependencyInjection();
                IServiceCollection services = new ServiceCollection();
                dependencyInjection.ConfigureServices(services);
                IServiceProvider serviceProvider = services.BuildServiceProvider();

                appSettings = dependencyInjection.AppSettings;

                _logCommand = serviceProvider.GetService<ILogCommand>();
                _telemetryClient = serviceProvider.GetService<TelemetryClient>();
                _jobService = serviceProvider.GetService<IJobService>();
                _jobRepository = serviceProvider.GetService<IJobRepository>();

                using (var operation = _telemetryClient.StartOperation<RequestTelemetry>("Running ARB Preview."))
                {
                    try
                    {
                        var msg = $"Starting StartProcessing ";
                        var log = MyLogger.GetDefaultLog(Guid.Empty, msg, -1, LogLevel.Info);
                        _logCommand.Execute(log);

                        _jobService.StartProcessing(appSettings);

                        msg = $"Completed StartProcessing ";
                        log = MyLogger.GetDefaultLog(Guid.Empty, msg, -1, LogLevel.Info);
                        _logCommand.Execute(log);
                    }
                    catch (Exception ex)
                    {
                        _telemetryClient.TrackException(ex);
                        throw;
                    }
                    _telemetryClient.StopOperation(operation);
                    _telemetryClient.Flush();
                }
            }
            catch (LoaderJobException ex)
            {
                var body = $"{ex}";
                Email.SendMail(body, appSettings.EmailSubject, appSettings.EmailFrom, appSettings.EmailTo, appSettings.EmailSmptServer);
                var job = GetJob(_jobRepository);
                AddLog(_logCommand, body, ex, job);
            }
            catch (Exception ex)
            {
                var body = $"{ex}";
                Email.SendMail(body, appSettings.EmailSubject, appSettings.EmailFrom, appSettings.EmailTo, appSettings.EmailSmptServer);

                var job = GetJob(_jobRepository);
                if (job == null)
                {
                    job = new Job()
                    {
                        StatusId = JobStatus.PREVIEW_JOB_FAILED,
                        StartDate = DateTime.Now,
                    };
                    _jobRepository.InsertJob(job);
                }
                else
                {
                    _jobService?.CompleteJob(job, false);
                }

                AddLog(_logCommand, body, ex, job);
            }
        }

        private static void AddLog(ILogCommand _logCommand, string msg, Exception ex, Job job)
        {
            var jobId = job?.Id ?? -1;
            var log = MyLogger.GetDefaultLog(Guid.Empty, msg, job.Id, LogLevel.Fatal);
            log.JobID = jobId;
            log.Exception = ex.ToString();
            _logCommand.Execute(log);
        }

        private static Job GetJob(IJobRepository jobRepository)
        {
            var job = jobRepository.GetJobListWithStatus()
                                    .OrderByDescending(j => j.Id)
                                    .FirstOrDefault();
            return job;
        }
    }
}

