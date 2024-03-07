using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.XForms.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(DirectionService))]
namespace Scavenger.XForms.Services
{
    public class DirectionChangedEventArgs
    {
        public DirectionChangedEventArgs(double lastHeading, double averageHeading)
        {
            LastHeading = lastHeading;
            AverageHeading = averageHeading;
        }

        public double LastHeading { get; set; }
        public double AverageHeading { get; set; }
    }
    
    public class DirectionService : IDirectionService
    {
        private IHeadingService _headingService;
        private double _lastHeading;
        private Queue<double> _recentHeadings = new Queue<double>(10);

        public event Action<DirectionChangedEventArgs> OnDirectionChanged;

        public DirectionService()
        {
            _headingService = DependencyService.Get<IHeadingService>();
            _headingService.HeadingChanged += headingService_HeadingChanged;
        }

        private void headingService_HeadingChanged(object sender, HeadingEventArgs e)
        {
            UpdateDirection(e.AzimuthRadians);
        }
        
        private void UpdateDirection(double azimuthRadians)
        {
                lock (this)
            {
                var sectorCount = 6;
                var sectorSize = (2 * Math.PI) / sectorCount;

                var sector = Math.Ceiling((azimuthRadians + Math.PI) / sectorSize);

                var heading = (sectorSize * sector) - Math.PI;

                //if (heading.Equals(_lastHeading))
                //    return;

                _recentHeadings.Enqueue(heading);

                var averageHeading = _recentHeadings.Average();

                //if (heading % averageHeading > 2)
                //    return;

                _lastHeading = heading;

                OnDirectionChanged?.Invoke(new DirectionChangedEventArgs(azimuthRadians, averageHeading));
            }
        }

        public bool IsListening { get; set; }

        public void StartListening()
        {
            _headingService.StartListening();
            IsListening = true;
        }

        public void StopListening()
        {
            _headingService.StopListening();
            IsListening = false;
        }
    }
}
