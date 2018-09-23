using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Scavenger.Server.Domain;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IScavengerLeaderboardGrain : IGrainWithIntegerKey
    {
        Task<Leaderboard> ScavengerFoundEgg(EggFoundResult result);
    }
}
