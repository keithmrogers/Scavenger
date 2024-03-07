using Orleans;
using Scavenger.Server.Domain;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyManagerGrain : IGrainWithIntegerKey
    {
        Task AddLobbyWaitingForScavenger(Guid lobbyId);
        Task AddLobbyWaitingForGuide(Guid lobbyId);
        Task<Lobby> GuideJoinLobby();
        Task<Lobby> ScavengerJoinLobby();
    }
}
