using System;
using System.IO;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Acquaint.XForms.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp
					.Android
					.ApkFile ("../../../Droid/bin/Release/com.xamarin.acquaint.forms-Signed.apk")
					.StartApp();
            }

            return ConfigureApp
				.iOS
				.AppBundle ("../../../iOS/bin/iPhoneSimulator/Debug/AcquaintXFormsiOS.app")
				.StartApp();
        }
    }
}

