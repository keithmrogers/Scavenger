

using Dapr.Actors.Runtime;
using Scavenger.Core;
using Scavenger.Interfaces;

namespace Scavenger.Actors;

public class LobbyManagerActor(ActorHost host) : Actor(host), ILobbyManagerActor
{
    private List<Guid> _lobbiesWaitingForGuides = [];
    private List<Guid> _lobbiesWaitingForScavengers = [];

    protected override Task OnActivateAsync()
    {
        _lobbiesWaitingForGuides = new List<Guid>();
        _lobbiesWaitingForScavengers = new List<Guid>();
        return Task.CompletedTask;
    }

    public async Task AddLobbyWaitingForGuide(Guid lobbyId)
    {
        _lobbiesWaitingForGuides.Add(lobbyId);
        await Task.CompletedTask;
    }

    public async Task AddLobbyWaitingForScavenger(Guid lobbyId)
    {
        _lobbiesWaitingForScavengers.Add(lobbyId);
        await Task.CompletedTask;
    }

    public async Task RemoveLobby(Guid lobbyId)
    {
        if (_lobbiesWaitingForGuides.Contains(lobbyId))
        {
            _lobbiesWaitingForGuides.Remove(lobbyId);
        }
        if (_lobbiesWaitingForScavengers.Contains(lobbyId))
        {
            _lobbiesWaitingForScavengers.Remove(lobbyId);
        }
        await Task.CompletedTask;
    }

    public async Task<Lobby> GuideJoinLobby()
    {
        var lobbyId = _lobbiesWaitingForGuides.Any() ? _lobbiesWaitingForGuides.First() : Guid.NewGuid();
        var lobbyActor = ProxyFactory.CreateActorProxy<ILobbyActor>(lobbyId.ToActorId(), nameof(LobbyActor));

        var lobby = await lobbyActor.GuideJoin();
        if (lobby.IsWaitingForScavenger)
        {
            await AddLobbyWaitingForScavenger(lobbyId);
        }
        if (lobby.IsReady)
        {
            await RemoveLobby(lobbyId);
            await StartGame(lobby.ScavengerId!.Value, lobby.GuideId!.Value);
        }
        return lobby;
    }

    public async Task<Lobby> ScavengerJoinLobby()
    {
        var lobbyId = _lobbiesWaitingForScavengers.Any() ? _lobbiesWaitingForScavengers.First() : Guid.NewGuid();
        var lobbyActor = ProxyFactory.CreateActorProxy<ILobbyActor>(lobbyId.ToActorId(), nameof(LobbyActor));

        var lobby = await lobbyActor.ScavengerJoin();
        if (lobby.IsWaitingForGuide)
        {
            await AddLobbyWaitingForGuide(lobbyId);
        }
        if (lobby.IsReady)
        {
            await RemoveLobby(lobbyId);
            await StartGame(lobby.ScavengerId!.Value, lobby.GuideId!.Value);
        }
        return lobby;
    }

    private async Task StartGame(Guid scavengerId, Guid guideId)
    {
        var gameId = Guid.NewGuid();
        var gameActor = ProxyFactory.CreateActorProxy<IGameActor>(gameId.ToActorId(), nameof(GameActor));

        await gameActor.Start(scavengerId, guideId);
    }
}
