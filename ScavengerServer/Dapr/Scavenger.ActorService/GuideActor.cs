using Dapr.Actors.Runtime;
using Scavenger.Interfaces;

namespace Scavenger.ActorService
{
    public class GuideActor(ActorHost host) : Actor(host), IGuideActor
    {
        private Guid gameId;

        public Task<Guid> GetGameId()
        {
            return Task.FromResult(gameId);
        }

        public Task SetGameId(Guid gameId)
        {
            this.gameId = gameId;

            return Task.CompletedTask;
        }
    }
}
