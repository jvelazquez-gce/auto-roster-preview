using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Constants;
using Domain.Exceptions;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using Services.Calculators;
using Domain.Entities;
using Domain.Configuration;
using Infrastructure.Utilities;
using Infrastructure.Database.Context;
using Newtonsoft.Json;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;
using Domain.Interfaces.Infrastructure.Database.Commands;

namespace Services.Jobs
{
    public class JobService : IJobService
    {
        AppSettings _appSettings = new AppSettings();
        private readonly IJobRepository _jobRepository;
        private readonly ILoaderValidator _loaderValidator;
        private readonly IDeleteAllLastClassGroupMissingFromPreLoadCommand _deleteAllLastClassGroupMissingFromPreLoadCommand;
        private readonly IPreRunDBSetUpCommand _preRunDBSetUpCommand;
        private readonly IBalancer _balancer;
        private readonly ISectionErrorRepository _sectionErrorRepository;
        private readonly IConfigInstance _configInstance;
        private readonly ICalculator _calculator;

        public JobService(IJobRepository jobRepository,
            ILoaderValidator loaderValidator,
            IDeleteAllLastClassGroupMissingFromPreLoadCommand deleteAllLastClassGroupMissingFromPreLoadCommand,
            IPreRunDBSetUpCommand preRunDBSetUpCommand,
            IBalancer balancer,
            ISectionErrorRepository sectionErrorRepository,
            IConfigInstance configInstance,
            ICalculator calculator)
        {
            _jobRepository = jobRepository;
            _loaderValidator = loaderValidator;
            _deleteAllLastClassGroupMissingFromPreLoadCommand = deleteAllLastClassGroupMissingFromPreLoadCommand;
            _preRunDBSetUpCommand = preRunDBSetUpCommand;
            _balancer = balancer;
            _sectionErrorRepository = sectionErrorRepository;
            _configInstance = configInstance;
            _calculator = calculator;
        }

        public void StartProcessing(AppSettings appSettings)
        {
            _appSettings = appSettings;

            _loaderValidator.CheckAndThrowExceptionIfNotSafeToRunBalancer();
            _deleteAllLastClassGroupMissingFromPreLoadCommand.ExecuteCommand();

            var foundStudentsToProcess = RunLiveAndPreviewJobs();
            if (!foundStudentsToProcess) throw new NoStudentsFoundException();
        }

        public bool RunLiveAndPreviewJobs()
        {
            var studentsFound = false;
            var job = new Job();
            for (var currentJob = 0; currentJob < 2; currentJob++)
            {
                var jobMode = currentJob == 0 ? JobStatus.RUNNING_LIVE_JOB : JobStatus.PREVIEW_JOB_RUNNING;
                job.StatusId = jobMode;
                job.StartDate = DateTime.Now;
                // TODO: need to find a way to dispose of the new ARBDb being injected
                var context = new ARBDb(_appSettings.DbConnectionString);
                var p = GetNewlyCreatedParameters(job, _appSettings, context);

                if (currentJob == 0)
                {
                    job.ProcessID = Guid.Empty;
                    _jobRepository.InsertJob(job);

                    var msg = $"RunLiveAndPreviewJobs has started";
                    var log = MyLogger.GetDefaultLog(Guid.Empty, msg, job.Id, LogLevel.Info);
                    p.LogCommand.Execute(log);

                    double daysWhenToStartDeletingCourses = _configInstance.GetNumberOfDaysWhenToStartDeletingCourses();
                    var dateWhenToStartDeletingCourses = DateTime.Today.AddDays(-daysWhenToStartDeletingCourses);

                    var queryInputParams = new QueryInputParams
                    {
                        Job = job, 
                        DateToStartDeletingCourses = dateWhenToStartDeletingCourses, 
                        CutOffStartDateTime = _configInstance.GetCutOffStartDateTime(),
                        CutOffEndDateTime = _configInstance.GetCutOffEndDateTime()
                    };

                    _preRunDBSetUpCommand.Execute(queryInputParams);
                }
                else
                {
                    // update status id
                    _jobRepository.UpdateJob(job);
                }

                var initialStudentRawData = new DataMapper().GetStudentDataWithGroupTypes(p, (int)job.StatusId);

                var courseStartDateList = _calculator.GetCourseStartDateListToProcess(initialStudentRawData);
                var courseStartDateListjson = JsonConvert.SerializeObject(courseStartDateList);
                var courseStartDateListJsonMsg = $"CourseStartDateListToProcess: {courseStartDateListjson}";
                MyLogger.CreateLog(p, courseStartDateListJsonMsg, -1, new DateTime(1800, 1, 1), LogLevel.Info, job.Id, job.ProcessID);

                foreach (var courseStartDate in courseStartDateList)
                {
                    var sectionsWithSameCourseAndStartDate = initialStudentRawData
                        .Where(l => l.AdCourseID == courseStartDate.CourseID)
                        .Where(l => l.StartDate == courseStartDate.StartDate)
                        .Distinct()
                        .ToList();

                    if (sectionsWithSameCourseAndStartDate.Any() && job.Id == 0)
                    {
                        job.ProcessID = sectionsWithSameCourseAndStartDate.FirstOrDefault().ProcessID;
                        _jobRepository.UpdateJob(job);
                    }

                    var msg = $"Processing course id: {courseStartDate.CourseID} start date: {courseStartDate.StartDate}";
                    MyLogger.CreateLog(p, msg, courseStartDate.CourseID, courseStartDate.StartDate, LogLevel.Info, job.Id, job.ProcessID);

                    studentsFound = true;

                    if (sectionsWithSameCourseAndStartDate.Any())
                    {
                        //p.AddPreviewStudentAndUpdateCourseSectionsCommand = new AddPreviewStudentAndUpdateCourseSectionsCommand(_appSettings.DbConnectionString);
                        //p.AddLiveModeStudentAndCourseSectionsCommand = new AddLiveModeStudentAndCourseSectionsCommand(_appSettings.DbConnectionString);
                        //var sectionErrorRepository = new SectionErrorRepository(_appSettings.DbConnectionString);

                        _balancer.ExcecuteLogicForUniqueCourseAndStartDate(
                            courseStartDate.CourseID, 
                            courseStartDate.StartDate,
                            sectionsWithSameCourseAndStartDate,
                            job,
                            p);
                    }
                }
            }
            if (job.Id != 0) CompleteJob(job, true);
            return studentsFound;
        }

        public void CompleteJob(Job job, bool jobCompletedSuccessfully)
        {
            job.CompletionDate = DateTime.Now;

            switch (job.StatusId)
            {
                case JobStatus.PREVIEW_JOB_RUNNING:
                    job.StatusId = jobCompletedSuccessfully 
                        ? JobStatus.PREVIEW_JOB_COMPLETED 
                        : JobStatus.PREVIEW_JOB_FAILED;
                    break;
                case JobStatus.RUNNING_LIVE_JOB:
                    job.StatusId = jobCompletedSuccessfully 
                        ? JobStatus.LIVE_JOB_COMPLETED 
                        : JobStatus.LIVE_JOB_FAILED;
                    break;
                default:
                    throw new Exception("Invalid Job Status.");
            }
            _jobRepository.UpdateJob(job);
        }

        private Parameters GetNewlyCreatedParameters(
            Job job,
            AppSettings appSettings,
            ARBDb context)
        {
            var p = new Parameters();
            p.AppSettings = appSettings;
            //p.LogCommand = new LogCommand(appSettings.DbConnectionString);
            //p.GetPreviewOneStudentPerSectionStudentsToTransferQuery = new GetPreviewOneStudentPerSectionStudentsToTransferQuery(appSettings.DbConnectionString);
            //p.GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery = new GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery(appSettings.DbConnectionString);
            //p.GetActiveOneStudentPerSectionCourseSectionsToCreateQuery = new GetActiveOneStudentPerSectionCourseSectionsToCreateQuery(appSettings.DbConnectionString);
            //p.GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery = new GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery(appSettings.DbConnectionString);
            //p.IsFeatureFlagOnQuery = new IsFeatureFlagOnQuery(appSettings.DbConnectionString);
            //p.GetPreLoadDataForPreviewQuery = new GetPreLoadDataForPreviewQuery(appSettings.DbConnectionString);
            //p.GetPreLoadDataForUpdateQuery = new GetPreLoadDataForUpdateQuery(appSettings.DbConnectionString);
            //p.GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery = new GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery(appSettings.DbConnectionString);
            //p.GetClassesTogetherToGroupRulesQuery = new GetClassesTogetherToGroupRulesQuery(appSettings.DbConnectionString);

            //p.UpdateOneStudentPerSectionRecordsToInactivateCommand = new UpdateOneStudentPerSectionRecordsToInactivateCommand(appSettings.DbConnectionString);
            //p.UpdateCourseSectionWithoutSavingCommand = new UpdateCourseSectionWithoutSavingCommand(context);
            //p.RemoveCourseSectionsWithoutSavingThemCommand = new RemoveCourseSectionsWithoutSavingThemCommand(context);
            //p.AddStudentSectionErrorCommand = new AddStudentSectionErrorCommand(appSettings.DbConnectionString);
            //p.AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand = new AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand(appSettings.DbConnectionString);
            //p.AddCourseSectionsCommand = new AddCourseSectionsCommand(appSettings.DbConnectionString);
            //p.SaveChangesCommand = new SaveChangesCommand(context);
            //p.UpdateCourseSectionCommand = new UpdateCourseSectionCommand(appSettings.DbConnectionString);
            p.QueryInputParams = new QueryInputParams()
            {
                CutOffStartDateTime = _configInstance.GetCutOffStartDateTime(),
                CutOffEndDateTime = _configInstance.GetCutOffEndDateTime(),
            };

            p.Job = job;
            p.PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue = new List<PreLoadStudentSection>();
            p.OneStudentPerSectionsWithSameCourseAndStartDateFromCVue = new List<OneStudentPerSectionRecord>();
            p.OneStudentPerSectionRecordFromDb = new List<OneStudentPerSectionRecord>();
            p.ListOfRecordsFromDbMissingMatchingRecordFromCVue = new List<OneStudentPerSectionRecord>();
            p.OneStudentPerSectionRecordToCreate = new List<OneStudentPerSectionRecord>();
            p.OneStudentPerSectionCourseSectionsReadyToCreateOrCreated = new List<CourseSection>();

            // Last Class Together
            //p.GetLastClassGroupStudentsToTransferQuery = new GetLastClassGroupStudentsToTransferQuery(appSettings.DbConnectionString);
            //p.AddLastClassGroupStudentSectionsCommand = new AddLastClassGroupStudentSectionsCommand(appSettings.DbConnectionString);
            //p.RemoveLastClassGroupStudentSectionCommand = new RemoveLastClassGroupStudentSectionCommand(appSettings.DbConnectionString);
            //p.GetActiveLastClassGroupCourseSectionsToCreateQuery = new GetActiveLastClassGroupCourseSectionsToCreateQuery(appSettings.DbConnectionString);
            //p.AddStudentSectionErrorsCommand = new AddStudentSectionErrorsCommand(appSettings.DbConnectionString);

            // Rules Engine
            //p.GetActiveRulesByCourseIdQuery = new GetActiveRulesByCourseIdQuery(appSettings.DbConnectionString);
            //p.GetClassGroupStudentsToTransferQuery = new GetClassGroupStudentsToTransferQuery(appSettings.DbConnectionString);
            //p.RemoveClassGroupStudentSectionCommand = new RemoveClassGroupStudentSectionCommand(appSettings.DbConnectionString);
            //p.AddClassGroupStudentSectionsCommand = new AddClassGroupStudentSectionsCommand(appSettings.DbConnectionString);
            //p.GetActiveClassGroupCourseSectionsToCreateQuery = new GetActiveClassGroupCourseSectionsToCreateQuery(appSettings.DbConnectionString);

            return p;
        }
    }
}
