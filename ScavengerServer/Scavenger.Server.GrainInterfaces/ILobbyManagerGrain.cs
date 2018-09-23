using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyManagerGrain : IGrainWithIntegerKey
    {
        Task AddLobbyWaitingForScavenger(Guid lobbyId);
        Task AddLobbyWaitingForGuide(Guid lobbyId);

        Task RemoveLobby(Guid lobbyId);

        Task GuideJoinLobby(ILobbyObserver lobbyObserver, IGuideObserver scavengerObserver);
        Task ScavengerJoinLobby(ILobbyObserver lobbyObserver, IScavengerObserver scavengerObserver);
    }
}
