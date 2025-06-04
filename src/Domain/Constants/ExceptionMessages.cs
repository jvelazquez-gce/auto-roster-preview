namespace Domain.Constants
{
    public static class ExceptionMessages
    {
        public const string NO_LOADER_JOB_FOUND_EXCEPTION_MSG = "No Loader job found!";
        public const string LOADER_JOB_RUNNING_EXCEPTION_MSG = "The Loader job is still running!";
        public const string LOADER_JOB_FAILED_EXCEPTION_MSG = "The Loader job failed!";
        public const string LOADER_JOB_COMPLETED_OUTSIDE_ALLOWED_LIMIT_EXCEPTION_MSG = "No Loader job completed within the allowed minutes!";
        public const string LOADER_JOB_COMPLETED_TIME_IS_GREATER_THAN_ARB_BALANCER_START_TIME_MSG = "The loader completed time cannot be greater than the balancer start time!";
    }
}