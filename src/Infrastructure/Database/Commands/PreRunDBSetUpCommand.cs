using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;
using Domain.Models.Helper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database.Commands
{
    public class PreRunDBSetUpCommand : IPreRunDBSetUpCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public PreRunDBSetUpCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute(QueryInputParams queryInputParams)
        {
            using var context = _dbContextFactory.CreateDbContext();
            //context.Database.CommandTimeout = GeneralConfiguration.DB_QUERY_TIMEOUT_TIME_IN_SECONDS;
            var parameters = new Microsoft.Data.SqlClient.SqlParameter[]
            {
                new Microsoft.Data.SqlClient.SqlParameter("@JobID", queryInputParams.Job.Id),
                new Microsoft.Data.SqlClient.SqlParameter("@DateToDeleteCourses", queryInputParams.DateToStartDeletingCourses),
                new Microsoft.Data.SqlClient.SqlParameter("@CutOffStartDateTime", queryInputParams.CutOffStartDateTime),
                new Microsoft.Data.SqlClient.SqlParameter("@CutOffEndDateTime", queryInputParams.CutOffEndDateTime)
            };
            // the PreRunDBSetUp calls these sprocs: 
            // EXEC dbo.DeleteOldRelatedData @JobID, @DateToDeleteCourses
            // EXEC dbo.ResetForecastFlag @CutOffStartDateTime, @CutOffEndDateTime
            // EXEC dbo.OneStudentPerSectionInactivateRecordsWithMissingCVueSection @CutOffStartDateTime, @CutOffEndDateTime

            //context.Database.ExecuteSqlRaw("Exec dbo.PreRunDBSetUp @JobID, @DateToDeleteCourses, @CutOffStartDateTime, @CutOffEndDateTime", parameters);

            var rowsAffected = context.Database.ExecuteSqlRaw("Exec dbo.PreRunDBSetUp @JobID, @DateToDeleteCourses, @CutOffStartDateTime, @CutOffEndDateTime", parameters);
        }
    }
}
