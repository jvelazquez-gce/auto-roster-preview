using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Interfaces.Infrastructure.Utilities;
using Domain.Entities;

namespace Services.Jobs
{
    public class LoaderValidator : ILoaderValidator
    {
        private readonly IJobRepository _jobRepository;
        private readonly IFeatureToggle _featureToggle;

        public LoaderValidator(IJobRepository jobRepository,
            IFeatureToggle featureToggle)
        {
            _jobRepository = jobRepository;
            _featureToggle = featureToggle;
        }

        public void CheckAndThrowExceptionIfNotSafeToRunBalancer()
        {
            var isLastJobStatusFlagOn = _featureToggle.IsFeatureEnabled(Feature.LastJobStatusFlag);

            if (!isLastJobStatusFlagOn) return;

            var loaderJobStatus = new List<int>() { JobStatus.LOADER_JOB_RUNNING, JobStatus.LOADER_JOB_COMPLETED, JobStatus.LOADER_JOB_FAILED };
            var lastJob = _jobRepository.GetJobListWithStatus()
                .Where(w => loaderJobStatus.Contains((int)w.StatusId))
                .OrderByDescending(o => o.StartDate)
                .FirstOrDefault();

            if (lastJob == null) throw new LoaderJobException(ExceptionMessages.NO_LOADER_JOB_FOUND_EXCEPTION_MSG);

            if (lastJob.StatusId == JobStatus.LOADER_JOB_RUNNING) throw new LoaderJobException(ExceptionMessages.LOADER_JOB_RUNNING_EXCEPTION_MSG);

            if (lastJob.StatusId == JobStatus.LOADER_JOB_FAILED) throw new LoaderJobException(ExceptionMessages.LOADER_JOB_FAILED_EXCEPTION_MSG);

            if (lastJob.StatusId == JobStatus.LOADER_JOB_COMPLETED)
            {
                if (!DidTheLoaderRunWithInTheLastXMinutes(lastJob))
                {
                    throw new LoaderJobException(ExceptionMessages.LOADER_JOB_COMPLETED_OUTSIDE_ALLOWED_LIMIT_EXCEPTION_MSG);
                }
            }
        }

        private bool DidTheLoaderRunWithInTheLastXMinutes(Job lastJob)
        {
            // Elapsed from the beginning of the century to Thursday, 14 November 2019 18:21:
            //    595,448,498,171,000,000 nanoseconds
            //    5,954,484,981,710,000 ticks
            //    595,448,498.17 seconds
            //    9,924,141.64 minutes
            //    6,891 days, 18 hours, 21 minutes, 38 seconds

            var elapsedTicks = DateTime.Now.Ticks - ((DateTime)lastJob.CompletionDate).Ticks;
            var elapsedSpan = new TimeSpan(elapsedTicks);

            //var minutesDifference = DateTime.Now.Minute - ((DateTime)lastJob.CompletionDate).Minute;
            var minutesDifference = elapsedSpan.TotalMinutes;
            /*
                completionDate = 12/1/2020 4:40:00 pm
                now = 12/1/2020 4:40:30 pm
                minutesDifference = 0
             */

            if (minutesDifference < 0) throw new LoaderJobException(ExceptionMessages.LOADER_JOB_COMPLETED_TIME_IS_GREATER_THAN_ARB_BALANCER_START_TIME_MSG);

            return minutesDifference <= 3;
        }
    }
}
