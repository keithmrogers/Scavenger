using Dapr.Actors.Client;
using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Actors.Events
{
    public class ScavengerMoved(IActorProxyFactory actorProxyFactory)
: Endpoint<ScavengerPositionChangedEvent>
    {
        private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;

        public override void Configure()
        {
            Post("/game/events/scavenger-position-changed");
            Options(rb => rb.WithTopic("scavenger-pubsub", "scavengers", nameof(ScavengerPositionChangedEvent), 1));
        }

        public override async Task HandleAsync(ScavengerPositionChangedEvent req, CancellationToken ct)
        {
            await actorProxyFactory.CreateActorProxy<IGameActor>(req.GameId.ToActorId(), nameof(GameActor)).CheckFoundEgg();
            await SendOkAsync(ct);
        }
    }
}
