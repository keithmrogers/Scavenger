using Dapr.Actors.Runtime;
using Dapr.Client;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Actors
{
    public class ScavengerActor(ActorHost host, DaprClient client) : Actor(host), IScavengerActor
    {
        private Core.Scavenger? scavenger;
        private readonly DaprClient client = client;

        protected override Task OnActivateAsync()
        {
            scavenger = new Core.Scavenger(Id.GetId());
            return Task.CompletedTask;
        }

        public async Task Move(Position position)
        {
            scavenger!.Move(position);
            await DispatchEvents(scavenger);
        }

        public async Task ChangeDirection(double direction)
        {
            scavenger!.ChangeDirection(direction);
            await DispatchEvents(scavenger);
        }

        private async Task DispatchEvents(Entity entity)
        {
            await client.PublishDomainEvents("scavengers", entity);
            entity.DomainEvents.Clear();
        }

        public async Task<Core.Scavenger> GetScavenger()
        {
            return await Task.FromResult(scavenger!);
        }

        public Task Start(Guid gameId)
        {
            scavenger!.Start(gameId);
            return Task.CompletedTask;
        }
    }
}
