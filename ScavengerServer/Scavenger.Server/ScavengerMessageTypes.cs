using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server
{
    public enum ScavengerClientMessageType
    {
        LobbyReady = 1,
        EggFound = 2
    }
    public enum ScavengerServiceMessageType
    {
        Move = 1,
        ChangeDirection = 2,
        Start = 3
    }
}
