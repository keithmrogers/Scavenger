using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Scavenger.XForms.Droid.Services;
using Scavenger.XForms.Services;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(CloseApplicationService))]
namespace Scavenger.XForms.Droid.Services
{
    public class CloseApplicationService : ICloseApplicationService
    {
        public void Close()
        {
            var activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}