using Orleans;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyManagerGrain : IGrainWithIntegerKey
    {
        Task AddLobbyWaitingForScavenger(Guid lobbyId);
        Task AddLobbyWaitingForGuide(Guid lobbyId);

        Task RemoveLobby(Guid lobbyId);

        Task GuideJoinLobby(ILobbyObserver lobbyObserver);
        Task ScavengerJoinLobby(ILobbyObserver lobbyObserver);
    }
}
