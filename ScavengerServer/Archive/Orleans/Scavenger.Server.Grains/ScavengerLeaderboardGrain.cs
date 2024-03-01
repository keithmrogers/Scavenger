using Orleans;
using Orleans.Providers;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    [StorageProvider(ProviderName = "FileStore")]
    public class ScavengerLeaderboardGrain : Grain<Leaderboard>, IScavengerLeaderboardGrain
    {
        public async Task<Leaderboard> ScavengerFoundEgg(EggFoundResult result)
        {
            var speed = result.Distance / result.TimeSeconds;

            if (speed > State.FastestEggFindSeconds)
            {
                State.FastestEggFindSeconds = speed;
            }
            if (result.Distance > State.FarthestDistanceBetweenEggFindsM)
            {
                State.FarthestDistanceBetweenEggFindsM = result.Distance;
            }
            if (result.TimeSeconds < State.ShortestTimeBetweenEggFindsSeconds)
            {
                State.ShortestTimeBetweenEggFindsSeconds = result.TimeSeconds;
            }
            await WriteStateAsync();

            return State;
        }
    }
}
