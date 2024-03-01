
using System.Runtime.Serialization;

namespace Scavenger.Core;

[DataContract]
public class EggFoundEvent : IDomainEvent
{
    public required Guid GameId { get; set; }
}
