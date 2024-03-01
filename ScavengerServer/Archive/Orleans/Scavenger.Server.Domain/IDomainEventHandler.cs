using System.Threading.Tasks;

namespace Scavenger.Server.Domain
{
    public interface IDomainEventHandler
    {
        Task HandleAsync(IDomainEvent domainEvent);
    }
}
