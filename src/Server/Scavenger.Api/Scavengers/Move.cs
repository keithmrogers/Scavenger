using Dapr.Actors.Client;
using FastEndpoints;
using Scavenger.Interfaces;

namespace Scavenger.Api.Scavengers
{
    public class Move(IActorProxyFactory actorProxyFactory)
  : Endpoint<MoveRequest>
    {
        private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;

        public override void Configure()
        {
            Patch("/api/scavenger/{ScavengerId}/move");
            AllowAnonymous();
        }

        public override async Task HandleAsync(MoveRequest req, CancellationToken ct)
        {
            var scavenger = actorProxyFactory.CreateActorProxy<IScavengerActor>(req.ScavengerId.ToActorId(), "ScavengerActor");
            await scavenger.Move(req.Position!);
        }
    }
}
