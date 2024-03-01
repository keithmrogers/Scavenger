using Orleans;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyObserver : IGrainObserver
    {
        Task LobbyReady(Guid scavengerId, Guid guideId);
    }
}
