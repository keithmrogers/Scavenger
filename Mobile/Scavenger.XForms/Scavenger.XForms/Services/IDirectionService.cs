using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.XForms.Services
{
    public interface IDirectionService
    {
        bool IsListening { get; }
        void StartListening();
        void StopListening();

        event Action<DirectionChangedEventArgs> OnDirectionChanged;
    }
}
