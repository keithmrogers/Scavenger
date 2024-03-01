
using Dapr.Actors.Runtime;
using Dapr.Client;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Actors;
public class LobbyActor(ActorHost host) : Actor(host), ILobbyActor
{
    private Lobby? lobby;

    protected override async Task OnActivateAsync()
    {
        var result = await StateManager.TryGetStateAsync<Lobby>("Lobby");
        if (result.HasValue)
        {
            lobby = result.Value;
        }
        else
        {
            lobby = new Lobby();
            await StateManager.AddStateAsync("Lobby", lobby);
            await SaveStateAsync();
        }
    }

    public async Task<Lobby> GuideJoin()
    {
        var guideId = Guid.NewGuid();
        var guideActor = ProxyFactory.CreateActorProxy<IGuideActor>(guideId.ToActorId(), nameof(GuideActor));
        lobby!.AddGuide(guideId);

        Console.WriteLine($"Guide {lobby.GuideId} joined Lobby {Id}");
        return await Task.FromResult(lobby);
    }

    public async Task<Lobby> ScavengerJoin()
    {
        var scavengerId = Guid.NewGuid();
        var scavengerActor = ProxyFactory.CreateActorProxy<IScavengerActor>(scavengerId.ToActorId(), nameof(ScavengerActor));
        lobby!.AddScavenger(scavengerId);

        Console.WriteLine($"Scavenger {lobby.ScavengerId} joined Lobby {Id}");
        return await Task.FromResult(lobby);
    }
}
