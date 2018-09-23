using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;

namespace Scavenger.Server.GrainInterfaces
{
    public interface ILobbyObserver : IGrainObserver
    {
        void LobbyReady(Guid scavengerId, Guid guideId);
    }
}
