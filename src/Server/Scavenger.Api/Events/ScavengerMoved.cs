using Dapr.Actors.Client;
using FastEndpoints;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Api.Events
{
    public class ScavengerMoved(IActorProxyFactory actorProxyFactory, IEventChannelManager channelManager)
: Endpoint<ScavengerPositionChangedEvent>
    {
        private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;
        private readonly IEventChannelManager channelManager = channelManager;

        public override void Configure()
        {
            Post("/events/scavenger-moved");
            Options(rb => rb.WithTopic("scavenger-pubsub", "scavengers", nameof(ScavengerPositionChangedEvent),2));
        }

        public override async Task HandleAsync(ScavengerPositionChangedEvent req, CancellationToken ct)
        {
            var gameActor = actorProxyFactory.CreateActorProxy<IGameActor>(req.GameId.ToActorId(), "GameActor");
            var guideId = await gameActor.GetGuideId();

            if (channelManager.TryGetWriter(guideId, out var writer))
            {
                await writer!.WriteAsync(req, ct);
            }
            await SendOkAsync(ct);
        }
    }
}
