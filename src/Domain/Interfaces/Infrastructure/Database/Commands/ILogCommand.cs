using Domain.Entities;

namespace Domain.Interfaces.Infrastructure.Database.Commands
{
    public interface ILogCommand
    {
        void Execute(Log log);
    }
}
