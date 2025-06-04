namespace Domain.Constants
{
    public static class JobStatus
    {
        public const int PREVIEW_JOB_RUNNING = 1;
        public const int PREVIEW_JOB_COMPLETED = 2;
        public const int RUNNING_LIVE_JOB = 3;
        public const int LIVE_JOB_COMPLETED = 4;
        public const int CLEAN_UP_SECTION_JOB_RUNNING = 5;
        public const int CLEAN_UP_SECTION_JOB_COMPLETED = 6;
        public const int PREVIEW_JOB_FAILED = 7;
        public const int LIVE_JOB_FAILED = 8;
        public const int CLEAN_UP_SECTION_JOB_FAILED = 9;
        public const int LOADER_JOB_RUNNING = 10;
        public const int LOADER_JOB_COMPLETED = 11;
        public const int LOADER_JOB_FAILED = 12;
    }
}
