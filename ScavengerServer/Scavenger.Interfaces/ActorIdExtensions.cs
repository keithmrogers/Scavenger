using Dapr.Actors;

namespace Scavenger.Interfaces
{
    public static class ActorIdExtensions
    {
        public static ActorId ToActorId(this Guid id)
        {
            return new ActorId(id.ToString());
        }
    }
}
