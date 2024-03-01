using Dapr.Actors;
using Dapr.Actors.Client;
using FastEndpoints;
using Scavenger.Interfaces;

namespace Scavenger.Api.Scavengers
{
    public class ChangeDirection(IActorProxyFactory actorProxyFactory)
  : Endpoint<ChangeDirectionRequest>
    {
        private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;

        public override void Configure()
        {
            Patch("/api/scavenger/{ScavengerId}/change-direction");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ChangeDirectionRequest req, CancellationToken ct)
        {
            var scavenger = actorProxyFactory.CreateActorProxy<IScavengerActor>(req.ScavengerId.ToActorId(), "ScavengerActor");
            await scavenger.ChangeDirection(req.Direction);
        }
    }
}
