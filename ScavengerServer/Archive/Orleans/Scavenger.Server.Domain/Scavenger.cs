using System;

namespace Scavenger.Server.Domain
{
    public class Scavenger(Guid scavengerId, Guid gameId) : Entity
    {
        private Position _position;

        public Position Position
        {
            get { return _position; }
            private set
            {
                if (Position != null && Position.Equals(value)) return;
                _position = value;
                AddDomainEvent(new ScavengerPositionChangedEvent(ScavengerId, GameId, Position));
            }
        }

        public Guid ScavengerId { get; private set; } = scavengerId;
        public Guid GameId { get; private set; } = gameId;

        private double _direction;
        public double Direction
        {
            get { return _direction; }
            private set
            {
                if (Direction.Equals(value)) return;
                _direction = value;
                AddDomainEvent(new ScavengerDirectionChangedEvent(_direction));
            }
        }

        public DateTime StartTime { get; set; }
        public Position StartPosition { get; set; }

        public void ChangeDirection(double direction)
        {
            Direction = direction;
        }

        public void Start()
        {
            StartPosition = new Position(0, 0);
            StartTime = DateTime.Now;
        }

        public void Move(Position location)
        {
            Position = location;
        }
    }
}
