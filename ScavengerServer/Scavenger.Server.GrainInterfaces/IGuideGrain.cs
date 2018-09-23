using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

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
