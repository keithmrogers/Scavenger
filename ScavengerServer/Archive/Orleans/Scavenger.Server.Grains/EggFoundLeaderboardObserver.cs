using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Grains
{
    public class EggFoundLeaderboardObserver: Grain, IAsyncObserver<EggFoundEvent>
    {
        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public async Task OnNextAsync(EggFoundEvent item, StreamSequenceToken token = null)
        {            
            var leaderboardGrain = GrainFactory.GetGrain<IScavengerLeaderboardGrain>(0);

            await leaderboardGrain.ScavengerFoundEgg(item.Result);
        }
    }
}
