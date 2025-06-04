using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Domain.Interfaces.Infrastructure.Database.Repositories;
using Domain.Models.Helper;
using System.Collections.Generic;

namespace Services.OneStudentPerSection
{
    public interface IOneStudentPerSectionHandler
    {
        OneStudentPerSectionResults ProcessRecordsAndGetCalcModel(List<PreLoadStudentSection> scVueSctionsWithSameCourseAndStartDate,
            IConfigurationRepository configurationRepository, CalcModel calcModel, Parameters p);

        void UpdateParameterObjectValues(CalcModel calcModel, Parameters p);

        void LoadDataToProcessAndRemoveRecordsFromListPassedInAndRunErrorChecks(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            Parameters p, IConfigurationRepository configurationRepository);

        List<PreLoadStudentSection> FindMixRecordsSharingSameSection(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate, Parameters p);

        // The value sectionsWithSameCourseAndStartDate gets updated and returned. If the returned values is not used beyond this method
        // that will be an issue since that parameter only gets updated in the scope of this method and not outside.
        List<PreLoadStudentSection> CheckForRecordsWithDataErrorsAndRemoveThem(List<PreLoadStudentSection> sectionsWithSameCourseAndStartDate,
            IAddStudentSectionErrorCommand AddStudentSectionErrorCommand, Job job, IConfigurationRepository configurationRepository,
            List<PreLoadStudentSection> mixRecordsSharingSameSection);

        OneStudentPerSectionResults ExcecuteLogic(Parameters p, IConfigurationRepository configurationRepository);

        void SetUpListsToCreateCancelAndRemove(Parameters p);

        void UpdateStudentRecordsWithNewSectionName(Parameters p);

        int GetNextCalculatedSectionNameBaseCounter(Parameters p);
    }
}
