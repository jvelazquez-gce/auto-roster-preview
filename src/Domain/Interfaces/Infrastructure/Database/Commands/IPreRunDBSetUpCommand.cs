using Domain.Models.Helper;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface IPreRunDBSetUpCommand
    {
        void Execute(QueryInputParams queryInputParams);
    }
}
