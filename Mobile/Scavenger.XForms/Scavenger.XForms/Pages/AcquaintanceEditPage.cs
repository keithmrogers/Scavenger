using System.ComponentModel;
using Scavenger.XForms.ViewModels;
using Xamarin.Forms;

namespace Scavenger.XForms.Pages
{
    public partial class AcquaintanceEditPage : ContentPage
    {
        protected AcquaintanceDetailViewModel ViewModel => BindingContext as AcquaintanceDetailViewModel;

        public AcquaintanceEditPage()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.iOS)
                Title = null; // because iOS already displays the previous page's title with the back button, we don't want to display it twice.
        }

        /// <summary>
        /// Ensures the state field has 2 characters at most.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The PropertyChangedEventArgs</param>
        void StateEntry_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                var entryCell = sender as EntryCell;

                string val = entryCell.Text;

                if (val.Length > 2)
                {
                    val = val.Remove(val.Length - 1);
                }

                entryCell.Text = val.ToUpperInvariant();
            }
        }

        /// <summary>
        /// Ensures the zip code field has 5 characters at most.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The PropertyChangedEventArgs</param>
        void PostalCode_PropertyChanged (object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Text")
            {
                var entryCell = sender as EntryCell;

                string val = entryCell.Text;

                if (val.Length > 5)
                {
                    val = val.Remove(val.Length - 1);
                    entryCell.Text = val;
                }
            }
            
        }
    }
}

