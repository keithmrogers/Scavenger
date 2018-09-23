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
    public class ScavengerGrain : Grain, IScavengerGrain
    {
        private Domain.Scavenger _scavenger;

        private ObserverSubscriptionManager<IGuideObserver> _guideObservers;

        private ObserverSubscriptionManager<IScavengerObserver> _scavengerObservers;

        public override Task OnActivateAsync()
        {
            this._guideObservers = new ObserverSubscriptionManager<IGuideObserver>();
            this._scavengerObservers = new ObserverSubscriptionManager<IScavengerObserver>();
            _scavenger = new Domain.Scavenger();

            return base.OnActivateAsync();
        }

        public Task Move(Position position)
        {
            _scavenger.Move(position);
            _guideObservers.Notify(observer=>observer.ScavengerMoved(position));
            return TaskDone.Done;
        }

        //TODO: Make Egg Finding a function of the scavenger domain, not dictated by the guide
        public async Task FoundEgg()
        {
            var result = _scavenger.FoundEgg();

            var leaderboardGrain = GrainFactory.GetGrain<IScavengerLeaderboardGrain>(0);

            var leaderboard = await leaderboardGrain.ScavengerFoundEgg(result);

            _guideObservers.Notify(observer => observer.EggFound(leaderboard));
            _scavengerObservers.Notify(observer => observer.EggFound());
        }

        public Task ChangeDirection(double direction)
        {
            _scavenger.ChangeDirection(direction);
            _guideObservers.Notify(observer => observer.ScavengerChangedDirection(direction));
            return TaskDone.Done;
        }

        public Task SubscribeGuide(IGuideObserver observer)
        {
            this._guideObservers.Clear();
            this._guideObservers.Subscribe(observer);
            return TaskDone.Done;
        }
        public Task UnsubscribeGuide(IGuideObserver observer)
        {
            this._guideObservers.Unsubscribe(observer);
            return TaskDone.Done;
        }

        public Task SubscribeScavenger(IScavengerObserver observer)
        {
            _scavengerObservers.Clear();
            _scavengerObservers.Subscribe(observer);
            return TaskDone.Done;
        }
        public Task UnsubscribeScavenger(IScavengerObserver observer)
        {
            this._scavengerObservers.Unsubscribe(observer);
            return TaskDone.Done;
        }
    }
}
