
using System.Runtime.Serialization;

namespace Scavenger.Core
{
    [DataContract]
    public class ScavengerDirectionChangedEvent : IDomainEvent
    {
        public required Guid ScavengerId { get; set; }
        public required double Direction { get; set; }
        public required Guid GameId { get; set; }
    }
}
