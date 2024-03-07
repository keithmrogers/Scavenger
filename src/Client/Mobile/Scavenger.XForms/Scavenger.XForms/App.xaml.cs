using FormsToolkit;
using Scavenger.Data;
using Scavenger.XForms.Constants;
using Scavenger.XForms.Pages;
using Scavenger.XForms.Services;
using Scavenger.XForms.ViewModels;
using Xamarin.Forms;

//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace Scavenger.XForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            SubscribeToDisplayAlertMessages();

			if (Device.OS == TargetPlatform.Android)
			{
				MainPage = new SplashPage();
			}
			else
			{
				var navPage = 
					new NavigationPage(
						new AcquaintanceListPage() 
						{ 
							BindingContext = new AcquaintanceListViewModel(), 
							Title="Acquaintances" }) 
				{ 
					BarBackgroundColor = Color.FromHex("547799"),
					BarTextColor = Color.White
				};

				MainPage = navPage;
			}

        }

        /// <summary>
        /// Subscribes to messages for displaying alerts.
        /// </summary>
        static void SubscribeToDisplayAlertMessages()
        {
            MessagingService.Current.Subscribe<MessagingServiceAlert>(MessageKeys.DisplayAlert, async (service, info) =>
                {
                    var task = Current?.MainPage?.DisplayAlert(info.Title, info.Message, info.Cancel);
                    if (task != null)
                    {
                        await task;
                        info?.OnCompleted?.Invoke();
                    }
                });

            MessagingService.Current.Subscribe<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, async (service, info) =>
                {
                    var task = Current?.MainPage?.DisplayAlert(info.Title, info.Question, info.Positive, info.Negative);
                    if (task != null)
                    {
                        var result = await task;
                        info?.OnCompleted?.Invoke(result);
                    }
                });
        }

    }
}

