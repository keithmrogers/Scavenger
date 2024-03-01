using Orleans;
using Scavenger.Server.Domain;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyGrain : IGrainWithGuidKey
    {
        Task<Lobby> ScavengerJoin();
        Task<Lobby> GuideJoin();
    }
}
