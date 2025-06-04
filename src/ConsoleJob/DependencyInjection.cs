using Domain.Configuration;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;
using Domain.Interfaces.Infrastructure.Database.Queries;
using Domain.Interfaces.Infrastructure.Database.Queries.LastClassGroupSection;
using Domain.Interfaces.Infrastructure.Database.Queries.OneStudentPerSection;
using Domain.Interfaces.Infrastructure.Database.Queries.RulesEngine;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Interfaces.Infrastructure.Utilities;
using Infrastructure.Database.Commands;
using Infrastructure.Database.Commands.LastClassGroups;
using Infrastructure.Database.Context;
using Infrastructure.Database.Queries;
using Infrastructure.Database.Queries.LastClassGroupSection;
using Infrastructure.Database.Queries.OneStudentPerSection;
using Infrastructure.Database.Queries.RulesEngine;
using Infrastructure.Database.Repositories;
using Infrastructure.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Calculators;
using Services.CVue;
using Services.Jobs;
using Services.LastClassTogether;
using Services.OneStudentPerSection;
using Services.Utilities;

namespace ConsoleJob
{
    public class DependencyInjection
    {
        static IConfigurationRoot Configuration { get; set; }
        public AppSettings AppSettings { get; set; } = new AppSettings();

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            // Step 3: Register services with configuration (if needed)
            var appSettings = new AppSettings();
            Configuration.GetSection("AppSettings").Bind(appSettings);
            services.AddSingleton<AppSettings>(appSettings);
            AppSettings = appSettings;

            services.AddApplicationInsightsTelemetryWorkerService(Configuration);

            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddSingleton<IDbContextFactory>(sp =>
            {
                var connectionString = AppSettings.DbConnectionString;
                return new DbContextFactory(connectionString);
            });
            services.AddTransient<ILogCommand, LogCommand>();
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddSingleton<IConfigurationRepository, ConfigurationRepository>();
            services.AddSingleton<IFeatureToggle, FeatureToggle>();

            services.AddSingleton<IConfigInstance, ConfigInstance>();

            services.AddTransient<IAddLastClassGroupStudentSectionsCommand, AddLastClassGroupStudentSectionsCommand>();
            services.AddTransient<IDeleteAllLastClassGroupMissingFromPreLoadCommand, DeleteAllLastClassGroupMissingFromPreLoadCommand>();
            services.AddTransient<IDisableLastClassGroupEmptySectionsCommand, DisableLastClassGroupEmptySectionsCommand>();
            services.AddTransient<IRemoveLastClassGroupStudentSectionCommand, RemoveLastClassGroupStudentSectionCommand>();

            services.AddTransient<IAddCourseSectionsCommand, AddCourseSectionsCommand>();
            services.AddTransient<IAddCourseSectionsTxCommand, AddCourseSectionsTxCommand>();
            services.AddTransient<IAddLiveModeStudentAndCourseSectionsCommand, AddLiveModeStudentAndCourseSectionsCommand>();
            services.AddTransient<IAddNewSectionAndStudentRecordsForOneStudentPerSectionCommand, AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand>();
            services.AddTransient<IAddPreviewStudentAndUpdateCourseSectionsCommand, AddPreviewStudentAndUpdateCourseSectionsCommand>();
            services.AddTransient<IAddPreviewStudentAndUpdateCourseSectionsCommand, AddPreviewStudentAndUpdateCourseSectionsCommand>();
            services.AddTransient<IAddStudentSectionErrorCommand, AddStudentSectionErrorCommand>();
            services.AddTransient<IAddStudentSectionErrorsCommand, AddStudentSectionErrorsCommand>();
            services.AddTransient<IDeleteOrInactivateCVueSectionsTxCommand, DeleteOrInactivateCVueSectionsTxCommand>();
            services.AddTransient<IPreRunDBSetUpCommand, PreRunDBSetUpCommand>();
            services.AddTransient<IRemoveCourseSectionsWithoutSavingThemCommand, RemoveCourseSectionsWithoutSavingThemCommand>();
            services.AddTransient<ISaveChangesCommand, SaveChangesCommand>();
            services.AddTransient<IUpdateCourseSectionCommand, UpdateCourseSectionCommand>();
            services.AddTransient<IUpdateCourseSectionWithoutSavingCommand, UpdateCourseSectionWithoutSavingCommand>();
            services.AddTransient<IUpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand, UpdateOneStudentPerSectionRecordsFromLiveRecordsNoSaveCommand>();
            services.AddTransient<IUpdateOneStudentPerSectionRecordsToInactivateCommand, UpdateOneStudentPerSectionRecordsToInactivateCommand>();

            services.AddTransient<IGetActiveLastClassGroupCourseSectionsToCreateQuery, GetActiveLastClassGroupCourseSectionsToCreateQuery>();
            services.AddTransient<IGetClassesTogetherToGroupRulesQuery, GetClassesTogetherToGroupRulesQuery>();
            services.AddTransient<IGetLastClassGroupStudentsToTransferQuery, GetLastClassGroupStudentsToTransferQuery>();

            services.AddTransient<IGetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery, GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery>();
            services.AddTransient<IGetActiveOneStudentPerSectionCourseSectionsToCreateQuery, GetActiveOneStudentPerSectionCourseSectionsToCreateQuery>();
            services.AddTransient<IGetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery, GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery>();
            services.AddTransient<IGetPreviewOneStudentPerSectionStudentsToTransferQuery, GetPreviewOneStudentPerSectionStudentsToTransferQuery>();

            services.AddTransient<IGetActiveClassGroupCourseSectionsToCreateQuery, GetActiveClassGroupCourseSectionsToCreateQuery>();
            services.AddTransient<IGetActiveRulesByCourseIdQuery, GetActiveRulesByCourseIdQuery>();
            services.AddTransient<IGetClassGroupStudentsToTransferQuery, GetClassGroupStudentsToTransferQuery>();

            services.AddTransient<IGetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery, GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery>();
            services.AddTransient<IGetPreLoadDataForPreviewQuery, GetPreLoadDataForPreviewQuery>();
            services.AddTransient<IGetPreLoadDataForUpdateQuery, GetPreLoadDataForUpdateQuery>();

            services.AddTransient<IGetPreLoadDataForUpdateQuery, GetPreLoadDataForUpdateQuery>();

            services.AddTransient<IConfigurationRepository, ConfigurationRepository>();
            services.AddTransient<IJobRepository, JobRepository>();
            services.AddTransient<ISectionErrorRepository, SectionErrorRepository>();

            services.AddTransient<IOneStudentPerSectionCourseSectionHandler, OneStudentPerSectionCourseSectionHandler>();
            services.AddTransient<IOneStudentPerSectionCVueSectionsHandler, OneStudentPerSectionCVueSectionsHandler>();
            services.AddTransient<IOneStudentPerSectionErrorChecking, OneStudentPerSectionErrorChecking>();
            services.AddTransient<ICampusVueSetUpHandler, CampusVueSetUpHandler>();
            services.AddTransient<ILastClassTogetherHandler, LastClassTogetherHandler>();
            services.AddTransient<IOneStudentPerSectionHandler, OneStudentPerSectionHandler>();
            services.AddTransient<INoGroupCalculator, NoGroupCalculator>();
            services.AddTransient<IGroupCalculator, GroupCalculator>();
            services.AddTransient<IGeneralLastClassTogetherCalculator, GeneralLastClassTogetherCalculator>();
            services.AddTransient<ICVueSectionsCalculator, CVueSectionsCalculator>();
            services.AddTransient<ICourseSectionCalculator, CourseSectionCalculator>();
            services.AddTransient<IErrorChecking, ErrorChecking>();
            services.AddTransient<ICalculator, Calculator>();
            services.AddTransient<IBalancer, Balancer>();
            services.AddTransient<ILoaderValidator, LoaderValidator>();
            services.AddTransient<IJobService, JobService>();
        }
    }
}
