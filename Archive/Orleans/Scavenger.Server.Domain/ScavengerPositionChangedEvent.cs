using System;

namespace Scavenger.Server.Domain
{
    public class ScavengerPositionChangedEvent : IDomainEvent
    {
        public ScavengerPositionChangedEvent(Guid scavengerId, Guid gameId, Position position)
        {
            ScavengerId = scavengerId;
            GameId = gameId;
            Position = position;
        }

        public Guid ScavengerId { get; }
        public Guid GameId { get; set; }
        public Position Position { get; set; }
    }
}
