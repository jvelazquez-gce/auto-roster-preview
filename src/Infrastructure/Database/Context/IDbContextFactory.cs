namespace Infrastructure.Database.Context
{
    public interface IDbContextFactory
    {
        ARBDb CreateDbContext();
    }
}
