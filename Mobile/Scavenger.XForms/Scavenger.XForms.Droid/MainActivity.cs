using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace Scavenger.XForms.Droid
{
	[Activity (Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : FormsAppCompatActivity
	// inhertiting from FormsAppCompatActivity is imperative to taking advantage of Android AppCompat libraries
	{
		protected override void OnCreate (Bundle bundle)
		{
			// this line is essential to wiring up the toolbar styles defined in ~/Resources/layout/toolbar.axml
			FormsAppCompatActivity.ToolbarResource = Resource.Layout.toolbar;
			base.OnCreate (bundle);
			Forms.Init (this, bundle);
			FormsMaps.Init (this, bundle);
			LoadApplication (new App ());
		}
	}
}
