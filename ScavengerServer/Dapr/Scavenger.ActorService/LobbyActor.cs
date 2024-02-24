
using Dapr.Actors.Runtime;
using Dapr.Client;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.ActorService;
public class LobbyActor(ActorHost host) : Actor(host), ILobbyActor
{
    private Lobby? lobby;

    protected override async Task OnActivateAsync()
    {
        var result = await this.StateManager.TryGetStateAsync<Lobby>("Lobby");
        if (result.HasValue)
        {
            lobby = result.Value;
        }
        else
        {
            lobby = new Lobby();
            await this.StateManager.AddStateAsync("Lobby",lobby);
            await this.SaveStateAsync();
        }
    }

    public async Task<Lobby> GuideJoin()
    {
        var guideId = Guid.NewGuid();
        var guideActor = this.ProxyFactory.CreateActorProxy<IGuideActor>(guideId.ToActorId(), nameof(GuideActor));
        lobby!.AddGuide(guideId);

        Console.WriteLine($"Guide {lobby.GuideId} joined Lobby {this.Id}");
        return await Task.FromResult(lobby);
    }

    public async Task<Lobby> ScavengerJoin()
    {
        var scavengerId = Guid.NewGuid();
        var scavengerActor = this.ProxyFactory.CreateActorProxy<IScavengerActor>(scavengerId.ToActorId(), nameof(ScavengerActor));
        lobby!.AddScavenger(scavengerId);

        Console.WriteLine($"Scavenger {lobby.ScavengerId} joined Lobby {this.Id}");
        return await Task.FromResult(lobby);
    }
}
