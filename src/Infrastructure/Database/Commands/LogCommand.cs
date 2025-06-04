using Domain.Entities;
using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;

namespace Infrastructure.Database.Commands
{
    public class LogCommand : ILogCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public LogCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void Execute(Log log)
        {
            using var context = _dbContextFactory.CreateDbContext();
            {
                context.Logs.Add(log);
                context.SaveChanges();
            }
        }
    }
}
