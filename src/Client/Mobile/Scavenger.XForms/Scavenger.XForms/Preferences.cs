using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Data;
using Xamarin.Forms;

namespace Scavenger.XForms
{
    public class Preferences
    {
        public static double? HomePositionLatitude
        {
            get { return Application.Current.Properties.ContainsKey("HomePositionLatitude") ? Application.Current.Properties["HomePositionLatitude"] as double? : null; }
            set { Application.Current.Properties["HomePositionLatitude"] = value; }
        }
        public static double? HomePositionLongitude
        {
            get { return Application.Current.Properties.ContainsKey("HomePositionLongitude") ? Application.Current.Properties["HomePositionLongitude"] as double?:null; }
            set { Application.Current.Properties["HomePositionLongitude"] = value; }
        }

        public static async Task SavePreferences()
        {
            await Application.Current.SavePropertiesAsync();
        }
    }
}
