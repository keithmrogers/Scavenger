using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Data;
using Scavenger.XForms.ViewModels;
using Xamarin.Forms;

namespace Scavenger.XForms.Pages
{
    public partial class LostConnectionPage : ContentPage
    {
        protected LostConnectionViewModel ViewModel => BindingContext as LostConnectionViewModel;

        public LostConnectionPage()
        {
            InitializeComponent();

            // on Android, we use a floating action button, so clear the ToolBarItems collection
            if (Device.OS == TargetPlatform.Android)
            {
                ToolbarItems.Clear();
            }
        }
    }
}
