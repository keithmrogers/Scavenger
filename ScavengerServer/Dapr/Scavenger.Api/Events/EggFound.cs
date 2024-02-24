using Dapr.Actors.Client;
using FastEndpoints;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Api.Events;

public class EggFound(IActorProxyFactory actorProxyFactory, IEventChannelManager channelManager)
: Endpoint<EggFoundEvent>
{
    private readonly IEventChannelManager channelManager = channelManager;
    private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;

    public override void Configure()
    {
        Post("/events/egg-found");
        Options(rb => rb.WithTopic("pubsub", "games", nameof(EggFoundEvent),2));
    }

    public override async Task HandleAsync(EggFoundEvent req, CancellationToken ct)
    {
        var gameActor = actorProxyFactory.CreateActorProxy<IGameActor>(req.GameId.ToActorId(), "GameActor");

        var guideId = await gameActor.GetGuideId();
        if (channelManager.TryGetWriter(guideId, out var gw))
        {
            await gw!.WriteAsync(req, ct);
        }

        var scavengerId = await gameActor.GetScavengerId();
        if (channelManager.TryGetWriter(scavengerId, out var sw))
        {
            await sw!.WriteAsync(req, ct);
        }

        await SendOkAsync(ct);
    }
}
