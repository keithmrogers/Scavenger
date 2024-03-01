using Orleans;
using Scavenger.Server.Domain;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IScavengerLeaderboardGrain : IGrainWithIntegerKey
    {
        Task<Leaderboard> ScavengerFoundEgg(EggFoundResult result);
    }
}
