using Orleans;
using Scavenger.Server.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class GuideGrain : Grain, IGuideGrain
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
