using ObjCRuntime;
using Scavenger.XForms.iOS.Services;
using Scavenger.XForms.Services;

[assembly: Xamarin.Forms.Dependency (typeof (EnvironmentService))]

namespace Scavenger.XForms.iOS.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        #region IEnvironmentService implementation
        public bool IsRealDevice => Runtime.Arch == Arch.DEVICE;

        #endregion
    }
}

