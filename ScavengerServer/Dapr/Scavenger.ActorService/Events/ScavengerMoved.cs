using Dapr.Actors.Client;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.ActorService.Events
{
    public class ScavengerMoved(IActorProxyFactory actorProxyFactory)
: Endpoint<ScavengerMovedRequest>
    {
        private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;

        public override void Configure()
        {
            Post("/game/events/scavenger-position-changed");
            Options(rb => rb.WithTopic("pubsub", "scavengers", nameof(ScavengerPositionChangedEvent), 1));
        }

        public override async Task HandleAsync(ScavengerMovedRequest req, CancellationToken ct)
        {
            await actorProxyFactory.CreateActorProxy<IGameActor>(req.ScavengerPositionChangedEvent!.GameId.ToActorId(), nameof(GameActor)).CheckFoundEgg();
            await SendOkAsync(ct);
        }
    }
}
