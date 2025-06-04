namespace Services.Jobs
{
    public interface ILoaderValidator
    {
        void CheckAndThrowExceptionIfNotSafeToRunBalancer();
    }
}
