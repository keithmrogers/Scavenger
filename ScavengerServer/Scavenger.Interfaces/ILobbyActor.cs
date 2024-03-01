using Dapr.Actors;
using Scavenger.Core;

namespace Scavenger.Interfaces;

public interface ILobbyActor : IActor
{
    Task<Lobby> ScavengerJoin();
    Task<Lobby> GuideJoin();
}
