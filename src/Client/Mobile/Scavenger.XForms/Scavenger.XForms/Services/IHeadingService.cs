using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.XForms.Services
{
    public class HeadingEventArgs
    {
        public float AzimuthRadians { get; set; }
    }
    
    public interface IHeadingService
    {
        event EventHandler<HeadingEventArgs> HeadingChanged;

        void StartListening();
        void StopListening();
    }
}
