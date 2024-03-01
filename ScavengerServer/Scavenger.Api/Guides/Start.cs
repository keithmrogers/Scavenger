using Dapr.Actors;
using Dapr.Actors.Client;
using FastEndpoints;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Api.Guides;

public class Start(IActorProxyFactory actorProxyFactory, IEventChannelManager channelManager)
: EndpointWithoutRequest<StartResponse>
{
    private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;
    private readonly IEventChannelManager channelManager = channelManager;

    public override void Configure()
    {
        Get("/api/guide/start");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var lobbyManagerActor = actorProxyFactory.CreateActorProxy<ILobbyManagerActor>(new ActorId("0"), "LobbyManagerActor");
        var lobby = await lobbyManagerActor.GuideJoinLobby();
        var guideId = lobby.GuideId!.Value;

        if (lobby.IsReady)
        {
            await SendAsync(new StartResponse { GuideId = guideId });
            return;
        }

        //if it's not ready, wait for the signal
        var reader = channelManager.GetReader(guideId);
        try
        {
            var item = await reader.ReadAsync(ct);
            if (item is GameStartedEvent)
            {
                await SendAsync(new StartResponse { GuideId = guideId });
            }
        }
        finally
        {
            channelManager.RemoveChannel(guideId);
        }
    }
}
