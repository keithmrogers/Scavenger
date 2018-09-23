using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Scavenger.Server.Domain;

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
