using Dapr.Actors;
using Scavenger.Core;

namespace Scavenger.Interfaces;

public interface ILobbyManagerActor : IActor
{
    Task AddLobbyWaitingForScavenger(Guid lobbyId);
    Task AddLobbyWaitingForGuide(Guid lobbyId);
    Task<Lobby> GuideJoinLobby();
    Task<Lobby> ScavengerJoinLobby();
}
