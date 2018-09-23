using System.Threading.Tasks;
using Scavenger.Data;
using Scavenger.XForms.ViewModels;
using Xamarin.Forms;

namespace Scavenger.XForms.Pages
{
    /// <summary>
    /// Splash Page that is used on Androd only. iOS splash characteristics are NOT defined here, ub tn the iOS prject settings.
    /// </summary>
    public partial class SplashPage : ContentPage
    {
        protected SplashViewModel ViewModel => BindingContext as SplashViewModel;

        public SplashPage()
        {
            InitializeComponent();
            BindingContext = new SplashViewModel();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // await a new task
            await Task.Factory.StartNew(() =>
            {
                // delay for a few seconds on the splash screen
                ViewModel.ConnectCommand.Execute(null);

                if (ViewModel.ScavengerId.HasValue)
                {
                    var navPage = new NavigationPage(new HuntingPage { Title = "Online...", BindingContext = new HuntingViewModel(ViewModel.ScavengerId.Value) });

                    // if this is iOS set the nav bar text color
                    if (Device.OS == TargetPlatform.iOS)
                        navPage.BarTextColor = Color.White;

                    // on the main UI thread, set the MainPage to the navPage
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage = navPage;
                    });
                }
            });


        }
    }
}

