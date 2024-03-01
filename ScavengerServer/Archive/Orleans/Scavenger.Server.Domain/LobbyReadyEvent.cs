using System;

namespace Scavenger.Server.Domain
{
    public class LobbyReadyEvent : IDomainEvent
    {
        public LobbyReadyEvent(Guid scavengerId, Guid guideId)
        {
            this.ScavengerId = scavengerId;
            this.GuideId = guideId;
        }

        public Guid ScavengerId { get; set; }
        public Guid GuideId { get; set; }
    }
}
