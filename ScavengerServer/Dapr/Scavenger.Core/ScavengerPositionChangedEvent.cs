using System;
using System.Runtime.Serialization;

namespace Scavenger.Core
{
    [DataContract]
    public class ScavengerPositionChangedEvent : IDomainEvent
    {
        public required Guid ScavengerId { get; set;  }
        public required Guid GameId { get; set; }
        public required Position Position { get; set; }
    }
}
