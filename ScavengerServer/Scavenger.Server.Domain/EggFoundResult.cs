using Orleans;

namespace Scavenger.Server.Domain
{
    [GenerateSerializer]
    public class EggFoundResult
    {
        [Id(0)]
        public double Distance { get; internal set; }
        [Id(1)]
        public int TimeMs { get; internal set; }
    }
}
