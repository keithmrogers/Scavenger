using Orleans;

namespace Scavenger.Server.Domain
{
    [GenerateSerializer]
    public class EggFoundResult
    {
        [Id(0)]
        public double Distance { get; set; }
        [Id(1)]
        public int TimeSeconds { get; set; }
    }
}
