using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Data;
using Scavenger.XForms.Pages;
using Scavenger.XForms.Services;
using Xamarin.Forms;

namespace Scavenger.XForms.ViewModels
{
    public class HuntingViewModel : BaseNavigationViewModel
    {
        private readonly Guid _scavengerId;
        private bool _isHunting;
        private readonly IMovementService _movementService;
        private readonly IDirectionService _directionService;
        private readonly IScavengerService _scavengerService;
        private readonly IScavengerClient _scavengerClient;
        private readonly ISoundService _soundService;

        private DateTime? _directLastUpdated;

        public HuntingViewModel(Guid scavengerId)
        {
            _isHunting = false;
            _movementService = DependencyService.Get<IMovementService>();
            _directionService = DependencyService.Get<IDirectionService>();
            _scavengerService = DependencyService.Get<IScavengerService>();
            _soundService = DependencyService.Get<ISoundService>();
            _scavengerClient = DependencyService.Get<IScavengerClient>();

            _directionService.OnDirectionChanged += _directionService_OnDirectionChanged;
            _movementService.CurrentPositionChanged += _movementService_CurrentPointChanged;
            _scavengerClient.OnEggFound += Client_OnEggFound;
            _scavengerId = scavengerId;
            
            _scavengerService.OnConnected += ScavengerService_OnConnected;
            _scavengerService.OnDisconnected += ScavengerService_OnDisconnected;
            //var locationService = DependencyService.Get<ILocationService>();
            //locationService.LocationChanged += _locationService_LocationChanged;
        }

        private void Client_OnEggFound()
        {
            _soundService.PlaySound("EggFound.mp3");
        }

        private void _directionService_OnDirectionChanged(DirectionChangedEventArgs e)
        {
            if (_directLastUpdated.HasValue && (DateTime.Now - _directLastUpdated.Value).Milliseconds <= 300) return;
            Direction = e.LastHeading;

            _directLastUpdated = DateTime.Now;
            Task.Run(() => _scavengerService.ChangeDirection(_scavengerId, Direction));

            var degrees = (Direction < 0 ? Direction + 2 * Math.PI : Direction) * 180 / Math.PI;

            CompassString = $"{degrees:N2}°";
            BearingString = degrees <= 90 ? $"N{degrees:N}°E" :
                            degrees <= 180 ? $"S{180 - degrees:N}°E" :
                            degrees <= 270 ? $"S{degrees - 180:N}°W" :
                                             $"N{360 - degrees:N}°W";
        }

        //private void _locationService_LocationChanged(object sender, LocationEventArgs e)
        //{
        //    //this.LatitudeString = "Latitude: " + e.Latitude.ToString();
        //    //this.LongitudeString = "Longitude: " + e.Longitude.ToString();

        //    var updated = DateTime.Now;
        //    this.LastUpdatedString = "Updated: " + updated.ToString() + "(" + (updated - _lastUpdated).Seconds + ")";

        //    _lastUpdated = updated;
        //}

        private void _movementService_CurrentPointChanged(object sender, MovementEventArgs e)
        {
            Task.Run(() => _scavengerService.Move(_scavengerId, e.CurrentPosition));

            this.CurrentPointString = e.CurrentPosition.ToString();
            this.HomeLocationString = $"Home: {_movementService.HomePositionLatitude},{_movementService.HomePositionLongitude}";
        }


        private Command _startHuntingCommand;
        public Command StartHuntingCommand => _startHuntingCommand ?? (_startHuntingCommand = new Command(ExecuteStartHuntingCommand));

        private void ExecuteStartHuntingCommand()
        {
            if (_isHunting) return;
            StartHuntingCommand.ChangeCanExecute();

            StartHunting();

            _isHunting = true;
            StartHuntingCommand.ChangeCanExecute();
        }

        private void StartHunting()
        {
            _movementService.StartListening();
            _directionService.StartListening();
        }

        private double _direction;
        public double Direction
        {
            get { return _direction; }
            set
            {
                _direction = value;
                OnPropertyChanged();
            }
        }

        //private string _latitudeString;
        //public string LatitudeString
        //{
        //    get { return _latitudeString; }
        //    set
        //    {
        //        _latitudeString = value;
        //        OnPropertyChanged("LatitudeString");
        //    }
        //}
        //private string _longitudeString;
        //public string LongitudeString
        //{
        //    get { return _longitudeString; }
        //    set
        //    {
        //        _longitudeString = value;
        //        OnPropertyChanged("LongitudeString");
        //    }
        //}

        //private DateTime _lastUpdated;
        //private string _lastUpdatedString;
        //public string LastUpdatedString
        //{
        //    get { return _lastUpdatedString; }
        //    set
        //    {
        //        _lastUpdatedString = value;
        //        OnPropertyChanged("LastUpdatedString");
        //    }
        //}

        private string _currentPointString;
        public string CurrentPointString
        {
            get { return _currentPointString; }
            set
            {
                _currentPointString = value;
                OnPropertyChanged();
            }
        }

        
            private string _homeLocationString;
        public string HomeLocationString
        {
            get { return _homeLocationString; }
            set
            {
                _homeLocationString = value;
                OnPropertyChanged();
            }
        }

        private string _compassString;
        public string CompassString
        {
            get { return _compassString; }
            set
            {
                _compassString = value;
                OnPropertyChanged();
            }
        }
        private string _bearingString;
        private readonly INavigation _navigation;

        public string BearingString
        {
            get { return _bearingString; }
            set
            {
                _bearingString = value;
                OnPropertyChanged();
            }
        }


        private async void ScavengerService_OnDisconnected()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await PushModalAsync(new LostConnectionPage{BindingContext = new LostConnectionViewModel()});
            });
        }

        private static void ScavengerService_OnConnected()
        {
        }
    }
}
