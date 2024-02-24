using Dapr.Actors.Runtime;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.ActorService;

public class LeaderboardActor(ActorHost host) : Actor(host), ILeaderboardActor
{
    public async Task<Leaderboard> ScavengerFoundEgg(EggFoundResult result)
    {
        var leaderboard = await this.StateManager.GetStateAsync<Leaderboard>("leaderboard");
        var speed = result.Distance / result.TimeSeconds;

        if (speed > leaderboard.FastestEggFindSeconds)
        {
            leaderboard.FastestEggFindSeconds = speed;
        }
        if (result.Distance > leaderboard.FarthestDistanceBetweenEggFindsM)
        {
            leaderboard.FarthestDistanceBetweenEggFindsM = result.Distance;
        }
        if (result.TimeSeconds < leaderboard.ShortestTimeBetweenEggFindsSeconds)
        {
            leaderboard.ShortestTimeBetweenEggFindsSeconds = result.TimeSeconds;
        }
        await this.SaveStateAsync();

        return leaderboard;
    }
}
