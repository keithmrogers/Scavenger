using System.Linq;
using System.Threading.Tasks;
using FormsToolkit;
using MvvmHelpers;
using Plugin.Messaging;
using Scavenger.Data;
using Scavenger.Util.Extensions;
using Scavenger.XForms.Constants;
using Scavenger.XForms.Pages;
using Scavenger.XForms.Services;
using Xamarin.Forms;

namespace Scavenger.XForms.ViewModels
{
    public class AcquaintanceListViewModel : BaseNavigationViewModel
    {
        public AcquaintanceListViewModel()
        {
            _CapabilityService = DependencyService.Get<ICapabilityService>();

            DataSource = new AcquaintanceDataSource();

            SubscribeToSaveAcquaintanceMessages();

            SubscribeToDeleteAcquaintanceMessages();
        }

        // this is just a utility service that we're using in this demo app to mitigate some limitations of the iOS simulator
        readonly ICapabilityService _CapabilityService;

        readonly IDataSource<Acquaintance> DataSource;

		ObservableRangeCollection<Acquaintance> _Acquaintances;

        Command _LoadAcquaintancesCommand;

        Command _RefreshAcquaintancesCommand;

        Command _NewAcquaintanceCommand;

		public ObservableRangeCollection<Acquaintance> Acquaintances
        {
            get { return _Acquaintances ?? (_Acquaintances = new ObservableRangeCollection<Acquaintance>()); }
		    set
            {
                _Acquaintances = value;
                OnPropertyChanged("Acquaintances");
            }
        }

        /// <summary>
        /// Command to load acquaintances
        /// </summary>
        public Command LoadAcquaintancesCommand
        {
            get { return _LoadAcquaintancesCommand ?? (_LoadAcquaintancesCommand = new Command(async () => await ExecuteLoadAcquaintancesCommand())); }
        }

        async Task ExecuteLoadAcquaintancesCommand()
        {
			if (Acquaintances.Count < 1)
            {
                LoadAcquaintancesCommand.ChangeCanExecute();

                await FetchAcquaintances();

                LoadAcquaintancesCommand.ChangeCanExecute();
            }
        }

        public Command RefreshAcquaintancesCommand
        {
            get { 
                return _RefreshAcquaintancesCommand ?? (_RefreshAcquaintancesCommand = new Command(async () => await ExecuteRefreshAcquaintancesCommandCommand())); 
            }
        }

        async Task ExecuteRefreshAcquaintancesCommandCommand()
        {
            RefreshAcquaintancesCommand.ChangeCanExecute();

            await FetchAcquaintances();

            RefreshAcquaintancesCommand.ChangeCanExecute();
        }

        async Task FetchAcquaintances()
        {
            IsBusy = true;

			var acquaintances = await DataSource.GetItems (0, 1000);

			Acquaintances.Clear ();

			Acquaintances.AddRange (acquaintances);

            IsBusy = false;
        }

        /// <summary>
        /// Command to create new acquaintance
        /// </summary>
        public Command NewAcquaintanceCommand
        {
            get
            {
                return _NewAcquaintanceCommand ??
                    (_NewAcquaintanceCommand = new Command(async () => await ExecuteNewAcquaintanceCommand()));
            }
        }

        async Task ExecuteNewAcquaintanceCommand()
        {
            await PushAsync(new AcquaintanceEditPage() { BindingContext = new AcquaintanceDetailViewModel(new Acquaintance()) });
        }

        Command _DialNumberCommand;

        /// <summary>
        /// Command to dial acquaintance phone number
        /// </summary>
        public Command DialNumberCommand
        {
            get
            {
                return _DialNumberCommand ??
                (_DialNumberCommand = new Command((parameter) =>
                        ExecuteDialNumberCommand((string)parameter)));
            }
        }

        void ExecuteDialNumberCommand(string acquaintanceId)
        {
            if (string.IsNullOrWhiteSpace(acquaintanceId))
                return;

            var acquaintance = _Acquaintances.SingleOrDefault(c => c.Id == acquaintanceId);

            if (acquaintance == null)
                return;

            if (_CapabilityService.CanMakeCalls)
            {
                var phoneCallTask = MessagingPlugin.PhoneDialer;
                if (phoneCallTask.CanMakePhoneCall)
                    phoneCallTask.MakePhoneCall(acquaintance.Phone.SanitizePhoneNumber());
            }
            else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    { 
                        Title = "Simulator Not Supported",
                        Message = "Phone calls are not supported in the iOS simulator.",
                        Cancel = "OK"
                    });
            }
        }

        Command _MessageNumberCommand;

        /// <summary>
        /// Command to message acquaintance phone number
        /// </summary>
        public Command MessageNumberCommand
        {
            get
            {
                return _MessageNumberCommand ??
                (_MessageNumberCommand = new Command((parameter) =>
                        ExecuteMessageNumberCommand((string)parameter)));
            }
        }

        void ExecuteMessageNumberCommand(string acquaintanceId)
        {
            if (string.IsNullOrWhiteSpace(acquaintanceId))
                return;

            var acquaintance = _Acquaintances.SingleOrDefault(c => c.Id == acquaintanceId);

            if (acquaintance == null)
                return;     

            if (_CapabilityService.CanSendMessages)
            {
                var messageTask = MessagingPlugin.SmsMessenger;
                if (messageTask.CanSendSms)
                    messageTask.SendSms(acquaintance.Phone.SanitizePhoneNumber());
            }
            else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    { 
                        Title = "Simulator Not Supported",
                        Message = "Messaging is not supported in the iOS simulator.",
                        Cancel = "OK"
                    });
            }
        }

        Command _EmailCommand;

        /// <summary>
        /// Command to email acquaintance
        /// </summary>
        public Command EmailCommand
        {
            get
            {
                return _EmailCommand ??
                (_EmailCommand = new Command((parameter) =>
                        ExecuteEmailCommandCommand((string)parameter)));
            }
        }

        void ExecuteEmailCommandCommand(string acquaintanceId)
        {
            if (string.IsNullOrWhiteSpace(acquaintanceId))
                return;

            var acquaintance = _Acquaintances.SingleOrDefault(c => c.Id == acquaintanceId);

            if (acquaintance == null)
                return;

            if (_CapabilityService.CanSendEmail)
            {
                var emailTask = MessagingPlugin.EmailMessenger;
                if (emailTask.CanSendEmail)
                    emailTask.SendEmail(acquaintance.Email);
            }
            else
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    { 
                        Title = "Simulator Not Supported",
                        Message = "Email composition is not supported in the iOS simulator.",
                        Cancel = "OK"
                    });
            }
        }

        /// <summary>
        /// Subscribes to "SaveAcquaintance" messages
        /// </summary>
        void SubscribeToSaveAcquaintanceMessages()
        {
            // This subscribes to the "SaveAcquaintance" message, and then inserts or updates the clisnts accordingly
            MessagingService.Current.Subscribe<Acquaintance>(MessageKeys.SaveAcquaintance, async (service, acquaintance) =>
                {
                    IsBusy = true;

                    await DataSource.SaveItem(acquaintance);

                    await FetchAcquaintances();

                    IsBusy = false;
                });
        }

        /// <summary>
        /// Subscribes to "DeleteAcquaintance" messages
        /// </summary>
        void SubscribeToDeleteAcquaintanceMessages()
        {
            // This subscribes to the "DeleteAcquaintance" message, and then deletes the acquaintance accordingly
            MessagingService.Current.Subscribe<Acquaintance>(MessageKeys.DeleteAcquaintance, async (service, acquaintance) =>
                {
                    IsBusy = true;

                    await DataSource.DeleteItem(acquaintance.Id);

                    await FetchAcquaintances();

                    IsBusy = false;
                });
        }
    }
}

