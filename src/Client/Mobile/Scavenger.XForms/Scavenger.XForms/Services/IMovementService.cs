using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Data;
using Xamarin.Forms;

namespace Scavenger.XForms.Services
{
    public class MovementEventArgs
    {
        public Position CurrentPosition { get; set; }
    }
    public interface IMovementService
    {
        bool IsListening { get; }
        double? HomePositionLatitude { get; set; }
        double? HomePositionLongitude { get; set; }

        void StartListening();
        void StopListening();

        event EventHandler<MovementEventArgs> CurrentPositionChanged;
    }
}
