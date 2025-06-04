using System.Collections.Generic;
using Domain.Interfaces.Infrastructure.Database.Queries;
using Domain.Entities;
using Domain.Configuration;
using Domain.Interfaces.Infrastructure.Database.Queries.LastClassGroupSection;
using Domain.Interfaces.Infrastructure.Database.Queries.OneStudentPerSection;
using Domain.Interfaces.Infrastructure.Database.Queries.Configurations;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Domain.Interfaces.Infrastructure.Database.Commands.LastClassGroups;
using Domain.Interfaces.Infrastructure.Database.Queries.RulesEngine;
using Domain.Interfaces.Infrastructure.Database.Commands.RulesEngine;

namespace Domain.Models.Helper
{
    public class Parameters
    {
        public AppSettings AppSettings { get; set; }
        public IGetActiveLastClassGroupCourseSectionsToCreateQuery GetActiveLastClassGroupCourseSectionsToCreateQuery { get; set; }
        public IGetLastClassGroupStudentsToTransferQuery GetLastClassGroupStudentsToTransferQuery { get; set; }
        public IGetPreviewOneStudentPerSectionStudentsToTransferQuery GetPreviewOneStudentPerSectionStudentsToTransferQuery { get; set; }
        public IGetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery GetOneStudentPerSectionCourseSectionsReadyToCreateOrCreatedQuery { get; set; }
        public IGetActiveOneStudentPerSectionCourseSectionsToCreateQuery GetActiveOneStudentPerSectionCourseSectionsToCreateQuery { get; set; }
        public IGetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery GetActiveCVueOneStudentPerSectionCourseSectionsToCancelQuery { get; set; }
        public IIsFeatureFlagOnQuery IsFeatureFlagOnQuery { get; set; }
        public IGetActiveRulesByCourseIdQuery GetActiveRulesByCourseIdQuery { get; set; }
        public IGetClassGroupStudentsToTransferQuery GetClassGroupStudentsToTransferQuery { get; set; }
        public IGetActiveClassGroupCourseSectionsToCreateQuery GetActiveClassGroupCourseSectionsToCreateQuery { get; set; }
        public IGetPreLoadDataForPreviewQuery GetPreLoadDataForPreviewQuery { get; set; }
        public IGetPreLoadDataForUpdateQuery GetPreLoadDataForUpdateQuery { get; set; }
        public IGetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery GetCourseSectionsThatCanBeReUsedByCourseIdAndStartDateQuery { get; set; }
        public IGetClassesTogetherToGroupRulesQuery GetClassesTogetherToGroupRulesQuery { get; set; }

        public IUpdateOneStudentPerSectionRecordsToInactivateCommand UpdateOneStudentPerSectionRecordsToInactivateCommand { get; set; }
        public IUpdateCourseSectionWithoutSavingCommand UpdateCourseSectionWithoutSavingCommand { get; set; }
        public IRemoveCourseSectionsWithoutSavingThemCommand RemoveCourseSectionsWithoutSavingThemCommand { get; set; }
        public IAddStudentSectionErrorCommand AddStudentSectionErrorCommand { get; set; }
        public IAddNewSectionAndStudentRecordsForOneStudentPerSectionCommand AddNewSectionAndStudentRecordsForOneStudentPerSectionCommand { get; set; }
        public IAddCourseSectionsCommand AddCourseSectionsCommand { get; set; }
        public IAddCourseSectionsTxCommand AddCourseSectionsTxCommand { get; set; }
        public ISaveChangesCommand SaveChangesCommand { get; set; }
        public IRemoveLastClassGroupStudentSectionCommand RemoveLastClassGroupStudentSectionCommand { get; set; }
        public IAddLastClassGroupStudentSectionsCommand AddLastClassGroupStudentSectionsCommand { get; set; }
        public IAddStudentSectionErrorsCommand AddStudentSectionErrorsCommand { get; set; }
        public IRemoveClassGroupStudentSectionCommand RemoveClassGroupStudentSectionCommand { get; set; }
        public IAddClassGroupStudentSectionsCommand AddClassGroupStudentSectionsCommand { get; set; }
        public ILogCommand LogCommand { get; set; }
        public IAddPreviewStudentAndUpdateCourseSectionsCommand AddPreviewStudentAndUpdateCourseSectionsCommand { get; set; }
        public IAddLiveModeStudentAndCourseSectionsCommand AddLiveModeStudentAndCourseSectionsCommand { get; set; }
        public IDeleteOrInactivateCVueSectionsTxCommand DeleteOrInactivateCVueSectionsTxCommand { get; set; }
        public IUpdateCourseSectionCommand UpdateCourseSectionCommand { get; set; }

        public QueryInputParams QueryInputParams { get; set; } = new QueryInputParams();
        public CalcModel CalcModel { get; set; } = new CalcModel();
        public Job Job { get; set; } = new Job();
        public List<PreLoadStudentSection> PreLoadOneStudentPerSectionsWithSameCourseAndStartDateFromCVue { get; set; } = new List<PreLoadStudentSection>();
        public List<OneStudentPerSectionRecord> OneStudentPerSectionsWithSameCourseAndStartDateFromCVue { get; set; } = new List<OneStudentPerSectionRecord>();
        public List<OneStudentPerSectionRecord> OneStudentPerSectionRecordFromDb { get; set; } = new List<OneStudentPerSectionRecord>();
        public List<OneStudentPerSectionRecord> ListOfRecordsFromDbMissingMatchingRecordFromCVue { get; set; } = new List<OneStudentPerSectionRecord>();

        public List<OneStudentPerSectionRecord> OneStudentPerSectionRecordToCreate { get; set; } = new List<OneStudentPerSectionRecord>();
        public List<CourseSection> OneStudentPerSectionCourseSectionsReadyToCreateOrCreated { get; set; } = new List<CourseSection>();
        public List<ClassesTogetherToGroupRule> ClassesTogetherToGroupRuleList { get; set; } = new List<ClassesTogetherToGroupRule>();
    }
}
