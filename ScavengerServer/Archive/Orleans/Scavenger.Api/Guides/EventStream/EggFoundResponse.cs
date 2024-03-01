namespace Scavenger.Api.Guides.EventStream
{
    public class EggFoundResponse
    {
        public required double FastestEggFindMs { get; set; }
        public required double FarthestDistanceBetweenEggFindsM { get; set; }
        public required double ShortestTimeBetweenEggFindsMs { get; set; }
    }
}
