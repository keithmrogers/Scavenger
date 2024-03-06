using Dapr.Actors.Client;
using FastEndpoints;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Api.Events
{
    public class ScavengerChangedDirection(IActorProxyFactory actorProxyFactory, IEventChannelManager channelManager)
: Endpoint<ScavengerDirectionChangedEvent>
    {
        private readonly IEventChannelManager channelManager = channelManager;

        public override void Configure()
        {
            Post("/events/scavenger-changed-direction");
            Options(rb => rb.WithTopic("scavenger-pubsub", "scavengers", nameof(ScavengerDirectionChangedEvent),1));
        }

        public override async Task HandleAsync(ScavengerDirectionChangedEvent req, CancellationToken ct)
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
