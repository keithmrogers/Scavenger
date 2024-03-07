using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LocationService))]

namespace Scavenger.XForms.Droid.Services
{
    public class LocationService : ILocationService
    {
        private const int TwoMinutes = 1000 * 60 * 2;
        private static Location _previousBestLocation;

        LocationManager _locationManager;
        private LocationListener _locationListener;

        public event EventHandler<LocationEventArgs> LocationChanged;

        public void StartListening(long minTimeBetweenUpdates)
        {
            _locationManager = (LocationManager)Forms.Context.GetSystemService(Context.LocationService);
            _locationListener = new LocationListener(LocationChanged);
            _locationManager.RequestLocationUpdates(LocationManager.GpsProvider, minTimeBetweenUpdates, 0, _locationListener);
            _locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, minTimeBetweenUpdates, 0, _locationListener);
        }

        public void StopListening()
        {
            _locationManager.RemoveUpdates(_locationListener);
        }


        protected static bool IsBetterLocation(Location location, Location currentBestLocation)
        {
            if (currentBestLocation == null)
            {
                // A new location is always better than no location
                return true;
            }

            // Check whether the new location fix is newer or older
            var timeDelta = location.Time - currentBestLocation.Time;
            var isSignificantlyNewer = timeDelta > TwoMinutes;
            var isSignificantlyOlder = timeDelta < -TwoMinutes;
            var isNewer = timeDelta > 0;

            // If it's been more than two minutes since the current location, use the new location
            // because the user has likely moved
            if (isSignificantlyNewer)
            {
                return true;
                // If the new location is more than two minutes older, it must be worse
            }
            else if (isSignificantlyOlder)
            {
                return false;
            }

            // Check whether the new location fix is more or less accurate
            int accuracyDelta = (int)(location.Accuracy - currentBestLocation.Accuracy);
            var isLessAccurate = accuracyDelta > 0;
            var isMoreAccurate = accuracyDelta < 0;
            var isSignificantlyLessAccurate = accuracyDelta > 200;

            // Check if the old and new location are from the same provider
            var isFromSameProvider = IsSameProvider(location.Provider,
                    currentBestLocation.Provider);

            // Determine location quality using a combination of timeliness and accuracy
            if (isMoreAccurate)
            {
                return true;
            }
            else if (isNewer && !isLessAccurate)
            {
                return true;
            }
            else if (isNewer && !isSignificantlyLessAccurate && isFromSameProvider)
            {
                return true;
            }
            return false;
        }
        private static bool IsSameProvider(string provider1, string provider2)
        {
            if (provider1 == null)
            {
                return provider2 == null;
            }
            return provider1.Equals(provider2);
        }


        ~LocationService()
        {
            StopListening();
        }

        private class LocationListener : Java.Lang.Object, ILocationListener
        {
            private readonly EventHandler<LocationEventArgs> _locationChanged;
            public LocationListener(EventHandler<LocationEventArgs> locationChanged)
            {
                _locationChanged = locationChanged;
            }

            public void OnLocationChanged(Location location)
            {
                if (!IsBetterLocation(location, _previousBestLocation)) return;

                var args = new LocationEventArgs
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude
                };
                _locationChanged?.Invoke(this, args);
                _previousBestLocation = location;
            }

            public void OnProviderDisabled(string provider)
            {
            }

            public void OnProviderEnabled(string provider)
            {
            }

            public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
            {
            }
        }
    }
}