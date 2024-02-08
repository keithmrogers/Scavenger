using Orleans;
using Scavenger.Server.Domain;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IGuideObserver : IGrainObserver
    {
        Task ScavengerMoved(Position position);
        Task ScavengerChangedDirection(double direction);

        Task EggFound(Leaderboard leaderboard);
    }
}
