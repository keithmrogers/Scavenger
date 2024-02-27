using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Services
{
    public class ScavengerLobbyObserver : ILobbyObserver
    {
        private readonly Action<Guid> _onScavengerReady;

        public ScavengerLobbyObserver(Action<Guid> onScavengerReady)
        {
            _onScavengerReady = onScavengerReady;
        }

        public void LobbyReady(Guid scavengerId, Guid guideId)
        {
            _onScavengerReady(scavengerId);
        }
    }
}
