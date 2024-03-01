using System;

namespace Scavenger.Core
{
    public class Scavenger(Guid scavengerId) : Entity
    {
        public static Position StartPosition { get; } = new Position(0, 0);

        private Position _position = StartPosition;

        public Scavenger(string scavengerId) : this(Guid.Parse(scavengerId))
        {
        }

        public Position Position
        {
            get { return _position; }
            private set
            {
                if (Position.Equals(value)) return;
                _position = value;
                AddDomainEvent(new ScavengerPositionChangedEvent { ScavengerId = ScavengerId, GameId = GameId, Position = Position });
            }
        }

        public Guid ScavengerId { get; private set; } = scavengerId;
        public Guid GameId { get; private set; } = default;

        private double _direction;
        public double Direction
        {
            get { return _direction; }
            private set
            {
                if (Direction.Equals(value)) return;
                _direction = value;
                AddDomainEvent(new ScavengerDirectionChangedEvent { ScavengerId = ScavengerId, GameId = GameId, Direction = _direction });
            }
        }

        public DateTime StartTime { get; set; }

        public void ChangeDirection(double direction)
        {
            Direction = direction;
        }

        public void Start(Guid gameId)
        {
            GameId = gameId;
            Position = StartPosition;
            StartTime = DateTime.Now;
        }

        public void Move(Position location)
        {
            Position = location;
        }
    }
}
