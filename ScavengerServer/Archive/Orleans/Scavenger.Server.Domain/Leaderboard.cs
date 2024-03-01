using Orleans;

namespace Scavenger.Server.Domain
{
    [GenerateSerializer]
    public class Leaderboard
    {
        [Id(0)]
        public double FarthestDistanceBetweenEggFindsM { get; set; }
        [Id(1)]
        public double FastestEggFindSeconds { get; set; }
        [Id(2)]
        public double ShortestTimeBetweenEggFindsSeconds { get; set; }
    }
}
