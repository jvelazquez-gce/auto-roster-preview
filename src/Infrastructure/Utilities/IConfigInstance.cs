using Domain.Entities.Other;
using System;
using System.Collections.Generic;

namespace Infrastructure.Utilities
{
    public interface IConfigInstance
    {
        bool RunARBProcess();

        int DaysBeforeLiveJobRuns();

        DateTime GetCutOffStartDateTime();

        DateTime GetCutOffEndDateTime();

        int GetCohortMinSize();
        string GetDefaultInstructorEmail();
        int GetDefaultInstructorID();
        int MaxTransferRecordsPerBatch();
        int GetCMCMaxSectionsPerBatch();
        int GetNumberOfDaysWhenToStartDeletingCourses();
        int MaxNumberOfForecastSections();
        List<Configuration> GetConfigurationList();
    }
}
