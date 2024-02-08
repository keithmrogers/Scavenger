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
            var speed = result.Distance / result.TimeMs;

            if (speed > State.FastestEggFindMs)
            {
                State.FastestEggFindMs = speed;
            }
            if (result.Distance > State.FarthestDistanceBetweenEggFindsM)
            {
                State.FarthestDistanceBetweenEggFindsM = result.Distance;
            }
            if (result.TimeMs < State.ShortestTimeBetweenEggFindsMs)
            {
                State.ShortestTimeBetweenEggFindsMs = result.TimeMs;
            }
            await WriteStateAsync();

            return State;
        }
    }
}
