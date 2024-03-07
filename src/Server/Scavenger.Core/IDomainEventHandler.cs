using System.Threading.Tasks;

namespace Scavenger.Core
{
    public interface IDomainEventHandler
    {
        Task HandleAsync(IDomainEvent domainEvent);
    }
}
