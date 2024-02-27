using Orleans;
using Scavenger.Server.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class GuideGrain : Grain, IGuideGrain
    {
        private Guid _scavengerId;

        public Task SetScavenger(Guid scavengerId)
        {
            _scavengerId = scavengerId;

            return Task.CompletedTask;
        }

        public Task ScavengerFoundEgg()
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(_scavengerId);
            scavengerGrain.FoundEgg();

            return Task.CompletedTask;
        }

        public Task Subscribe(IGuideObserver guideObserver)
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(_scavengerId);
            scavengerGrain.SubscribeGuide(guideObserver);

            return Task.CompletedTask;
        }

        public Task Unsubscribe(IGuideObserver guideObserver)
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(_scavengerId);
            scavengerGrain.UnsubscribeGuide(guideObserver);

            return Task.CompletedTask;
        }
    }
}
