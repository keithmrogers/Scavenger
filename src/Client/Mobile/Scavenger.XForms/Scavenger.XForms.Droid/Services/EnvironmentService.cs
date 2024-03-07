using Android.OS;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.Services;

[assembly: Xamarin.Forms.Dependency (typeof (EnvironmentService))]

namespace Scavenger.XForms.Droid.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        #region IEnvironmentService implementation
        public bool IsRealDevice
        {
            get
            {
                string f = Build.Fingerprint;
                return !(f.Contains("vbox") || f.Contains("generic") || f.Contains("vsemu"));
            }
        }
        #endregion
    }
}

