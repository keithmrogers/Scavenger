using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.XForms.Services
{
    public class LocationEventArgs
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public interface ILocationService
    {
        event EventHandler<LocationEventArgs> LocationChanged;

        void StartListening(long minTimeBetweenUpdates);
        void StopListening();
    }
}
