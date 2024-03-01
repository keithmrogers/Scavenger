
namespace Scavenger.Server.Domain
{
    public class EggFoundEvent : IDomainEvent
    {
        public EggFoundEvent(EggFoundResult result)
        {
            Result = result;
        }

        public EggFoundResult Result { get; }
    }
}
