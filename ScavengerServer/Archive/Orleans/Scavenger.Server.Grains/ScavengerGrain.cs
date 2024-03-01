using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class ScavengerGrain : Grain, IScavengerGrain
    {
        private Domain.Scavenger _scavenger;
        private IAsyncStream<IDomainEvent> stream;
        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            var streamProvider = this.GetStreamProvider("SMSProvider");
            stream = streamProvider.GetStream<IDomainEvent>(StreamId.Create("Scavenger", this.GetPrimaryKey()));

            return base.OnActivateAsync(cancellationToken);
        }

        public async Task Move(Position position)
        {
            _scavenger.Move(position);
            await StreamEvents(_scavenger);
        }

        public async Task ChangeDirection(double direction)
        {
            _scavenger.ChangeDirection(direction);
            await StreamEvents(_scavenger);
        }

        private async Task StreamEvents(Entity entity)
        {
            await stream.OnNextBatchAsync(entity.DomainEvents);
        }

        public async Task<Domain.Scavenger> GetScavenger()
        {
            return await Task.FromResult(_scavenger);
        }

        public async Task<Guid> GetGameId()
        {
            return await Task.FromResult(_scavenger.GameId);
        }
    }
}
