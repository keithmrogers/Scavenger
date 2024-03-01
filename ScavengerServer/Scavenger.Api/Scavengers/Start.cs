using Dapr.Actors.Client;
using Dapr.Actors;
using FastEndpoints;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Api.Scavengers;

public class Start(IActorProxyFactory actorProxyFactory, IEventChannelManager channelManager)
: EndpointWithoutRequest<StartResponse>
{
    private readonly IActorProxyFactory actorProxyFactory = actorProxyFactory;
    private readonly IEventChannelManager channelManager = channelManager;

    public override void Configure()
    {
        Get("/api/scavenger/start");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var lobbyManagerActor = actorProxyFactory.CreateActorProxy<ILobbyManagerActor>(new ActorId("0"), "LobbyManagerActor");
        var lobby = await lobbyManagerActor.ScavengerJoinLobby();
        var scavengerId = lobby.ScavengerId!.Value;

        if (lobby.IsReady)
        {
            await SendAsync(new StartResponse { ScavengerId = scavengerId });
            return;
        }

        //if it's not ready, wait for the signal
        var reader = channelManager.GetReader(scavengerId);
        try
        {
            var item = await reader.ReadAsync(ct);
            if (item is GameStartedEvent)
            {
                await SendAsync(new StartResponse { ScavengerId = scavengerId });
            }
        }
        finally
        {
            channelManager.RemoveChannel(scavengerId);
        }
    }
}
