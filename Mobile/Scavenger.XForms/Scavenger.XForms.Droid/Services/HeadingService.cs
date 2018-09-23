using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Hardware;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MvvmHelpers;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(HeadingService))]
namespace Scavenger.XForms.Droid.Services
{
    public class HeadingService : IHeadingService
    {
        private SensorManager _sensorManager;
        private Sensor _accelerometer;
        private Sensor _magnetometer;
        private ISensorEventListener _sensorEventListener;
        
        public event EventHandler<HeadingEventArgs> HeadingChanged;
        

        public void StartListening()
        {
            if (_sensorManager == null)
                _sensorManager = (SensorManager)Forms.Context.GetSystemService(Context.SensorService);

            if (_accelerometer == null)
                _accelerometer = _sensorManager?.GetDefaultSensor(SensorType.Accelerometer);

            if (_magnetometer == null)
                _magnetometer = _sensorManager?.GetDefaultSensor(SensorType.MagneticField);

            if (_sensorEventListener == null)
                _sensorEventListener = new SensorEventListener(HeadingChanged);

            _sensorManager.RegisterListener(_sensorEventListener, _accelerometer, SensorDelay.Normal);
            _sensorManager.RegisterListener(_sensorEventListener, _magnetometer, SensorDelay.Normal);
        }

        public void StopListening()
        {
            _sensorManager.UnregisterListener(_sensorEventListener, _accelerometer);
            _sensorManager.UnregisterListener(_sensorEventListener, _magnetometer);
        }

        private class SensorEventListener : Java.Lang.Object, ISensorEventListener
        {
            private readonly object _locker = new object();
            private readonly float[] _lastAccelerometer = new float[3];
            private readonly float[] _lastMagnetometer = new float[3];
            private bool _lastAccelerometerSet;
            private bool _lastMagnetometerSet;
            private readonly float[] _r = new float[9];
            private readonly float[] _orientation = new float[3];

            private readonly EventHandler<HeadingEventArgs> _headingChanged;
            public SensorEventListener(EventHandler<HeadingEventArgs> headingChanged)
            {
                _headingChanged = headingChanged;
            }

            public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
            {
            }

            public void OnSensorChanged(SensorEvent e)
            {
                lock (_locker)
                {
                    if (e.Sensor.Type == SensorType.Accelerometer && !_lastAccelerometerSet)
                    {
                        CopyValues(e.Values, _lastAccelerometer);
                        _lastAccelerometerSet = true;
                    }
                    else if (e.Sensor.Type == SensorType.MagneticField && !_lastMagnetometerSet)
                    {
                        CopyValues(e.Values, _lastMagnetometer);
                        _lastMagnetometerSet = true;
                    }
                    if (_lastAccelerometerSet && _lastMagnetometerSet)
                    {
                        HeadingDetector(_lastAccelerometer, _lastMagnetometer);
                        _lastAccelerometerSet = false;
                        _lastMagnetometerSet = false;
                    }
                }
            }

            private void HeadingDetector(float[] accelerometer, float[] magnetometer)
            {
                lock (this)
                {
                    SensorManager.GetRotationMatrix(_r, null, accelerometer, magnetometer);
                    SensorManager.GetOrientation(_r, _orientation);
                    var azimuthInRadians = _orientation[0];

                    _headingChanged?.Invoke(this, new HeadingEventArgs { AzimuthRadians = azimuthInRadians });
                }
            }
            private void CopyValues(System.Collections.Generic.IList<float> source, float[] destination)
            {
                for (int i = 0; i < source.Count; ++i)
                {
                    destination[i] = source[i];
                }
            }
        }
    }
}