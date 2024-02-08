using Orleans;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class ScavengerGrain : Grain, IScavengerGrain
    {
        private Domain.Scavenger _scavenger;

        private HashSet<IGuideObserver> _guideObservers;

        private HashSet<IScavengerObserver> _scavengerObservers;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            this._guideObservers = new HashSet<IGuideObserver>();
            this._scavengerObservers = new HashSet<IScavengerObserver>();
            _scavenger = new Domain.Scavenger();

            return base.OnActivateAsync(cancellationToken);
        }

        public async Task Move(Position position)
        {
            _scavenger.Move(position);
            await Task.WhenAll(_guideObservers.Select(observer => observer.ScavengerMoved(position)));
        }

        //TODO: Make Egg Finding a function of the scavenger domain, not dictated by the guide
        public async Task FoundEgg()
        {
            var result = _scavenger.FoundEgg();

            var leaderboardGrain = GrainFactory.GetGrain<IScavengerLeaderboardGrain>(0);

            var leaderboard = await leaderboardGrain.ScavengerFoundEgg(result);

            await Task.WhenAll(_guideObservers.Select(observer => observer.EggFound(leaderboard)));
            await Task.WhenAll(_scavengerObservers.Select(observer => observer.EggFound()));
        }

        public async Task ChangeDirection(double direction)
        {
            _scavenger.ChangeDirection(direction);
            await Task.WhenAll(_guideObservers.Select(observer => observer.ScavengerChangedDirection(direction)));
        }

        public Task SubscribeGuide(IGuideObserver observer)
        {
            this._guideObservers.Clear();
            this._guideObservers.Add(observer);
            return Task.CompletedTask;
        }
        public Task UnsubscribeGuide(IGuideObserver observer)
        {
            this._guideObservers.Remove(observer);
            return Task.CompletedTask;
        }

        public Task SubscribeScavenger(IScavengerObserver observer)
        {
            _scavengerObservers.Clear();
            _scavengerObservers.Add(observer);
            return Task.CompletedTask;
        }
        public Task UnsubscribeScavenger(IScavengerObserver observer)
        {
            this._scavengerObservers.Remove(observer);
            return Task.CompletedTask;
        }
    }
}
