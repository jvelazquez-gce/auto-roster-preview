using System;

namespace Domain.Interfaces.Infrastructure.Utilities
{
    public interface IConfiguration
    {
        bool RunARBProcess();

        int DaysBeforeLiveJobRuns();

        DateTime GetCutOffStartDateTime();

        DateTime GetCutOffEndDateTime();

        int GetInclusiveMinSize();
        string GetDefaultInstructorEmail();
        int GetDefaultInstructorID();
        int MaxTransferRecordsPerBatch();
        int GetCMCMaxSectionsPerBatch();
    }
}
