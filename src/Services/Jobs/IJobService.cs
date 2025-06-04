using Domain.Configuration;
using Domain.Entities;

namespace Services.Jobs
{
    public interface IJobService
    {
        void StartProcessing(AppSettings appSettings);

        bool RunLiveAndPreviewJobs();

        void CompleteJob(Job job, bool jobCompletedSuccessfully);
    }
}
