using Domain.Interfaces.Infrastructure.Database.Commands;
using Infrastructure.Database.Context;

namespace Infrastructure.Database.Commands
{
    public class SaveChangesCommand : ISaveChangesCommand
    {
        private readonly IDbContextFactory _dbContextFactory;

        public SaveChangesCommand(IDbContextFactory dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void ExecuteCommand()
        {
            using var context = _dbContextFactory.CreateDbContext();
            context.SaveChanges();
        }
    }
}
