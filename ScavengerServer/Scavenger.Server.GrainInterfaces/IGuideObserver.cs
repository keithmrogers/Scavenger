using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Scavenger.Server.Domain;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IGuideObserver: IGrainObserver
    {
        void ScavengerMoved(Position position);
        void ScavengerChangedDirection(double direction);

        void EggFound(Leaderboard leaderboard);
    }
}
