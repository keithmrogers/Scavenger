using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using CoreLocation;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.iOS.Services;
using Scavenger.XForms.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LocationService))]

namespace Scavenger.XForms.Droid.Services
{
    public class LocationService : ILocationService
    {
        private const long MinimumDistanceChangeForUpdates = 1; // in Meters
        private const long MinimumTimeBetweenUpdates = 7000;

        LocationManager _locationManager;

        public event EventHandler<LocationEventArgs> LocationChanged;

        public void OnLocationChanged(CLLocationCoordinate2D location)
        {
            var args = new LocationEventArgs
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude
            };
            LocationChanged?.Invoke(this, args);
        }

        public void StartTracking()
        {
            if (CLLocationManager.LocationServicesEnabled)
            {
                _locationManager = new LocationManager();
                //set the desired accuracy, in meters
                _locationManager.LocMgr.DesiredAccuracy = MinimumDistanceChangeForUpdates;
                _locationManager.LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) =>
                {
                    // fire our custom Location Updated event
                    OnLocationChanged(e.Locations[e.Locations.Length - 1].Coordinate);
                };
                _locationManager.LocMgr.StartUpdatingLocation();
            }
        }
    }
}