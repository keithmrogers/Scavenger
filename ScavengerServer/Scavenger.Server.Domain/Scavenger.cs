using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server.Domain
{
    public class Scavenger : INotifyPropertyChanged
    {
        public Scavenger()
        {
        }
        private Position _position;
        private int _eggsCollected;

        private Position LastEggPosition { get; set; }

        private DateTime? LastEggFoundTime { get; set; }

        public Position Position
        {
            get { return _position; }
            private set
            {
                if (Position != null && Position.Equals(value)) return;
                _position = value;
                NotifyPropertyChanged("Position");
            }
        }

        private double _direction;
        public double Direction
        {
            get { return _direction; }
            private set
            {
                if (Direction.Equals(value)) return;
                _direction = value;
                NotifyPropertyChanged("Direction");
            }
        }

        public DateTime StartTime { get; set; }
        public Position StartPosition { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void ChangeDirection(double direction)
        {
            Direction = direction;
        }

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public EggFoundResult FoundEgg()
        {
            _eggsCollected++;
            var eggFoundTime = DateTime.Now;
            var result = new EggFoundResult
            {
                Distance = GetDistance(LastEggPosition ?? StartPosition, Position),
                TimeMs = (eggFoundTime - (LastEggFoundTime ?? StartTime)).Milliseconds
            };

            LastEggPosition = Position;
            LastEggFoundTime = eggFoundTime;

            return result;
        }

        private static double GetDistance(Position lastEggPosition, Position position)
        {
            var a = (double)(position.X - lastEggPosition.X);
            var b = (double)(position.Y - lastEggPosition.Y);

            return Math.Sqrt(a * a + b * b);
        }

        public void Move(Position location)
        {
            if (Position == null)
            {
                StartPosition = location;
                StartTime = DateTime.Now;
            }
            Position = location;
        }
    }
}
