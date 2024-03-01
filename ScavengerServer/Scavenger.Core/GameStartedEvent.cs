using System;
using System.Runtime.Serialization;

namespace Scavenger.Core
{
    [DataContract]
    public class GameStartedEvent : IDomainEvent
    {
        [DataMember]
        public required Guid ScavengerId { get; set; }
        [DataMember]
        public required Guid GameId { get; set; }
        [DataMember]
        public required Guid GuideId { get; set; }
    }
}
