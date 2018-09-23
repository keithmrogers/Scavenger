using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server.Domain
{
    public class Leaderboard
    {
        public double FarthestDistanceBetweenEggFindsM { get; set; }
        public double FastestEggFindMs { get; set; }
        public double ShortestTimeBetweenEggFindsMs { get; set; }
    }
}
