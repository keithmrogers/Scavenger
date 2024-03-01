using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server
{
    public enum GuideClientMessageType
    {
        LobbyReady = 1,
        ScavengerMoved = 2,
        ScavengerChangedDirection = 3,
        EggFound = 4
    }
    public enum GuideServiceMessageType
    {
        Start = 1,
        ScavengerFoundEgg = 2
    }
}
