using System.Runtime.Serialization;

namespace Scavenger.Core;

[DataContract]
public class Lobby : Entity
{
    [DataMember]
    public Guid? ScavengerId { get; private set; }

    [DataMember]
    public Guid? GuideId { get; private set; }

    public void AddScavenger(Guid scavengerId)
    {
        if (ScavengerId.HasValue)
        {
            throw new Exception("A scavenger has already joined the lobby");
        }

        ScavengerId = scavengerId;
    }

    public void AddGuide(Guid guideId)
    {
        if (GuideId.HasValue)
        {
            throw new Exception("A guide has already joined the lobby");
        }

        GuideId = guideId;
    }

    public bool IsReady { get { return ScavengerId.HasValue && GuideId.HasValue; } }

    public bool IsWaitingForGuide { get { return !GuideId.HasValue; } }

    public bool IsWaitingForScavenger { get { return !ScavengerId.HasValue; } }
}
