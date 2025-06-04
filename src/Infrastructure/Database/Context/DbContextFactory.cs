namespace Infrastructure.Database.Context
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly string _connectionString;

        public DbContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ARBDb CreateDbContext()
        {
            return new ARBDb(_connectionString);
        }
    }
}
