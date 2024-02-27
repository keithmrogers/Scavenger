using Orleans;
using Scavenger.Server.Domain;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IScavengerGrain : IGrainWithGuidKey
    {
        Task Move(Position position);
        Task ChangeDirection(double direction);
        Task SubscribeGuide(IGuideObserver observer);
        Task UnsubscribeGuide(IGuideObserver observer);

        Task SubscribeScavenger(IScavengerObserver observer);
        Task UnsubscribeScavenger(IScavengerObserver observer);
        Task FoundEgg();
    }
}
