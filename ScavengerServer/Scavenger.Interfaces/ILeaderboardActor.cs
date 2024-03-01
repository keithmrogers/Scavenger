using Dapr.Actors;
using Scavenger.Core;

namespace Scavenger.Interfaces;

public interface ILeaderboardActor : IActor
{
    Task<Leaderboard> ScavengerFoundEgg(EggFoundResult result);
}
