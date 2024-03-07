using Foundation;
using UIKit;

namespace Acquaint.Native.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		// The Window property. The root of the app's UI hierarchy.
		public override UIWindow Window { get; set; }

		// A reference to the Main.storyboard
	    private static readonly UIStoryboard Storyboard = UIStoryboard.FromName("Main", null);

		// Method invoked after the application has launched to configure the main window and view controller.
		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
		{
			// Override point for customization after application launch.
			// If not required for your application you can safely delete this method

			// intantiate a new instance of Window with the device's screen bounds
			Window = new UIWindow(UIScreen.MainScreen.Bounds);

			// Get an instance of the initial view controller from the storyboard. In our case, this is a UINavigationController.
			var initialViewController = Storyboard.InstantiateInitialViewController();

			// configure some style properties
			ConfigureStyle(initialViewController as UINavigationController);

			// Assign the initial view controller to Window's RootViewController property
			Window.RootViewController = initialViewController;

			// this makes the Window the primary window in the iOS window hierarchy and displays it
			Window.MakeKeyAndVisible();

			return true;
		}

		void ConfigureStyle(UINavigationController navigationController)
		{
			// setting the NavigationBar.BarStyle to UIBarStyle.Black makes the status bar text and icons white.
			navigationController.NavigationBar.BarStyle = UIBarStyle.Black;

			// set some stlye properties foe the navigation bar
			UINavigationBar.Appearance.TintColor = UIColor.White;
			UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(84, 119, 153);
			UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes { ForegroundColor = UIColor.White };
			UIBarButtonItem.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White }, UIControlState.Normal);
		}

		public override void OnResignActivation(UIApplication application)
		{
			// Invoked when the application is about to move from active to inactive state.
			// This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
			// or when the user quits the application and it begins the transition to the background state.
			// Games should use this method to pause the game.
		}

		public override void DidEnterBackground(UIApplication application)
		{
			// Use this method to release shared resources, save user data, invalidate timers and store the application state.
			// If your application supports background exection this method is called instead of WillTerminate when the user quits.
		}

		public override void WillEnterForeground(UIApplication application)
		{
			// Called as part of the transiton from background to active state.
			// Here you can undo many of the changes made on entering the background.
		}

		public override void OnActivated(UIApplication application)
		{
			// Restart any tasks that were paused (or not yet started) while the application was inactive. 
			// If the application was previously in the background, optionally refresh the user interface.
		}

		public override void WillTerminate(UIApplication application)
		{
			// Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
		}
	}
}


