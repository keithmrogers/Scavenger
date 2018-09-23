using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Grains
{
    public class GuideGrain : Grain, IGuideGrain
    {
        private Guid _scavengerId;

        public Task SetScavenger(Guid scavengerId)
        {
            _scavengerId = scavengerId;

            return TaskDone.Done;
        }

        public Task ScavengerFoundEgg()
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(_scavengerId);
            scavengerGrain.FoundEgg();

            return TaskDone.Done;
        }

        public Task Subscribe(IGuideObserver guideObserver)
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(_scavengerId);
            scavengerGrain.SubscribeGuide(guideObserver);

            return TaskDone.Done;
        }

        public Task Unsubscribe(IGuideObserver guideObserver)
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(_scavengerId);
            scavengerGrain.UnsubscribeGuide(guideObserver);

            return TaskDone.Done;
        }
    }
}
