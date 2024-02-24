using System.Runtime.Serialization;

namespace Scavenger.Core;

[DataContract]
public abstract class Entity
{
    [DataMember]
    public List<IDomainEvent> DomainEvents { get; private set; } = [];

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        DomainEvents.Remove(eventItem);
    }
}