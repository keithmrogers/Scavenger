using Orleans;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IGuideGrain : IGrainWithGuidKey
    {
        Task SetScavenger(Guid scavengerId);

        Task ScavengerFoundEgg();
        Task Subscribe(IGuideObserver guideObserver);
        Task Unsubscribe(IGuideObserver guideObserver);
    }
}
