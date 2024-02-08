using Orleans;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyGrain : IGrainWithGuidKey
    {
        Task ScavengerJoin(ILobbyObserver observer);
        Task GuideJoin(ILobbyObserver observer);
    }
}
