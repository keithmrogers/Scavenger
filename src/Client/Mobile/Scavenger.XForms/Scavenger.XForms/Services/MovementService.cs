using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scavenger.XForms.Services;
using Xamarin.Forms;
using Scavenger.Data;

[assembly: Dependency(typeof(MovementService))]

namespace Scavenger.XForms.Services
{
    public class MovementService : IMovementService
    {
        private readonly ILocationService _locationService;

        private const double MinimumDistanceChangeForUpdatesKm = .001; // in KM
        private const long MinimumTimeBetweenUpdates = 500;

        public MovementService()
        {
            _locationService = DependencyService.Get<ILocationService>();
            _locationService.LocationChanged += _locationService_LocationChanged;

        }

        private void _locationService_LocationChanged(object sender, LocationEventArgs e)
        {
            Task.Run(() => UpdateLocation(e.Latitude, e.Longitude));
        }

        private Position _currentPosition;

        private Position CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                _currentPosition = value;
                CurrentPositionChanged?.Invoke(this, new MovementEventArgs() { CurrentPosition = _currentPosition });
            }
        }

        public bool IsListening { get; set; }

        public double? HomePositionLatitude { get; set; }

        public double? HomePositionLongitude { get; set; }

        private double _lastPositionLatitude;
        private double _lastPositionLongitude;

        private List<Position> _homePositionList;


        public event EventHandler<MovementEventArgs> CurrentPositionChanged;

        private async Task UpdateLocation(double latitude, double longitude)
        {
            if (!HomePositionLatitude.HasValue || !HomePositionLongitude.HasValue)
            {
                _homePositionList.Add(new Position(latitude, longitude));

                if (_homePositionList.Count < 10)
                {
                    return;
                }

                InitializeHomePosition(_homePositionList);

                CurrentPosition = new Position(0, 0);

                _lastPositionLatitude = latitude;
                _lastPositionLatitude = longitude;
                return;
            }

            var homePositionLatitude = HomePositionLatitude.Value;
            var homePositionLongitude = HomePositionLongitude.Value;

            var distLast = GetDistance(_lastPositionLatitude, _lastPositionLongitude, latitude,
                longitude);

            if (!(distLast > MinimumDistanceChangeForUpdatesKm)) return;

            var distH = GetDistance(homePositionLatitude, homePositionLongitude, latitude, longitude);

            var distX = GetDistance(homePositionLatitude, homePositionLongitude,
                homePositionLatitude, longitude);
            var distY = Math.Sqrt(Math.Pow(distH, 2) - Math.Pow(distX, 2));

            distX = (longitude < homePositionLongitude ? -1 : 1) * distX;
            distY = (latitude < homePositionLatitude ? -1 : 1) * distY;

            CurrentPosition = new Position(distX * 1000,
                distY * 1000);

            _lastPositionLatitude = latitude;
            _lastPositionLongitude = longitude;
        }

        private void InitializeHomePosition(IList<Position> _homePositionList)
        {
            var areas = new List<Position>();

            foreach (var position in _homePositionList)
            {
                var area = areas.FirstOrDefault(a => GetDistance(position.X, position.Y, a.X, a.Y) <= 0.03);
                areas.Add(area ?? position);
            }

            var mode = areas.GroupBy(p => new {p.X, p.Y}).OrderByDescending(g=>g.Count()).First();
            
            HomePositionLatitude = mode.Key.X;
            HomePositionLongitude = mode.Key.Y;
        }

        private double GetDistance(double latitudeA, double longitudeA, double latitudeB, double longtitudeB)
        {

            var latA = ToRadians(latitudeA);
            var lonA = ToRadians(longitudeA);
            var latB = ToRadians(latitudeB);
            var lonB = ToRadians(longtitudeB);
            var cosAng = (Math.Cos(latA) * Math.Cos(latB) * Math.Cos(lonB - lonA)) +
                            (Math.Sin(latA) * Math.Sin(latB));
            var ang = Math.Acos(cosAng);
            return ang * 6371;
        }

        private double ToRadians(double angleDegrees)
        {
            return (Math.PI / 180) * angleDegrees;
        }

        public void StartListening()
        {
            _locationService.StartListening(MinimumTimeBetweenUpdates);
            IsListening = true;
            HomePositionLatitude = null;
            HomePositionLongitude = null;
            _homePositionList = new List<Position>();
    }

        public void StopListening()
        {
            _locationService.StopListening();
            IsListening = false;
        }

    }
}