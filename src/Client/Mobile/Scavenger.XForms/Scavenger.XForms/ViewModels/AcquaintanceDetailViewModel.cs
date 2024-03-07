using System;
using System.Linq;
using System.Threading.Tasks;
using FormsToolkit;
using Plugin.ExternalMaps;
using Plugin.ExternalMaps.Abstractions;
using Plugin.Messaging;
using Scavenger.Data;
using Scavenger.Util.Extensions;
using Scavenger.XForms.Constants;
using Scavenger.XForms.Pages;
using Scavenger.XForms.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Scavenger.XForms.ViewModels
{
    public class AcquaintanceDetailViewModel : BaseNavigationViewModel
    {
        bool _IsNewAcquaintance;

        // this is just a utility service that we're using in this demo app to mitigate some limitations of the iOS simulator
        readonly ICapabilityService _CapabilityService;

        readonly Geocoder _Geocoder;

        public AcquaintanceDetailViewModel(Acquaintance acquaintance = null)
        {
            _CapabilityService = DependencyService.Get<ICapabilityService>();

            _Geocoder = new Geocoder();

            if (acquaintance == null)
            {
                _IsNewAcquaintance = true;
                Acquaintance = new Acquaintance();
            }
            else
            {
                _IsNewAcquaintance = false;
                Acquaintance = acquaintance;
            }

			Title = _IsNewAcquaintance ? "New Acquaintance" : _Acquaintance.DisplayLastNameFirst;

            _AddressString = Acquaintance.AddressString;

            SubscribeToSaveAcquaintanceMessages();

            SubscribeToAcquaintanceLocationUpdatedMessages();
        }

        public bool IsExistingAcquaintance => !_IsNewAcquaintance;

        public bool HasEmailAddress => !string.IsNullOrWhiteSpace(Acquaintance?.Email);

        public bool HasPhoneNumber => !string.IsNullOrWhiteSpace(Acquaintance?.Phone);

        public bool HasAddress => !string.IsNullOrWhiteSpace(Acquaintance?.AddressString);

        private string _AddressString;

        Acquaintance _Acquaintance;

        public Acquaintance Acquaintance
        {
            get { return _Acquaintance; }
            set
            {
                _Acquaintance = value;
				OnPropertyChanged("Acquaintance");
            }
        }

        Command _SaveAcquaintanceCommand;

        public Command SaveAcquaintanceCommand
        {
            get
            {
                return _SaveAcquaintanceCommand ?? (_SaveAcquaintanceCommand = new Command(() => ExecuteSaveAcquaintanceCommand()));
            }
        }

        async Task ExecuteSaveAcquaintanceCommand()
        {
            if (string.IsNullOrWhiteSpace(Acquaintance.LastName) || string.IsNullOrWhiteSpace(Acquaintance.FirstName))
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Invalid name!", 
                        Message = "A acquaintance must have both a first and last name.",
                        Cancel = "OK"
                    });
            }
            else if (!RequiredAddressFieldCombinationIsFilled)
            {
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                    {
                        Title = "Invalid address!", 
                        Message = "You must enter either a street, city, and state combination, or a postal code.",
                        Cancel = "OK"
                    });
            }
            else
            {
                MessagingService.Current.SendMessage<Acquaintance>(MessageKeys.SaveAcquaintance, Acquaintance);

                await PopAsync();
            }
        }

        bool RequiredAddressFieldCombinationIsFilled
        {
            get
            {
                if (Acquaintance.AddressString.IsNullOrWhiteSpace())
                {
                    return true;
                }
                if (!Acquaintance.Street.IsNullOrWhiteSpace() && !Acquaintance.City.IsNullOrWhiteSpace() && !Acquaintance.State.IsNullOrWhiteSpace())
                {
                    return true;
                }
                if (!Acquaintance.PostalCode.IsNullOrWhiteSpace() && (Acquaintance.Street.IsNullOrWhiteSpace() || Acquaintance.City.IsNullOrWhiteSpace() || Acquaintance.State.IsNullOrWhiteSpace()))
                {
                    return true;
                }

                return false;
            }
        }

        Command _EditAcquaintanceCommand;

        public Command EditAcquaintanceCommand
        {
            get
            {
                return _EditAcquaintanceCommand ??
                    (_EditAcquaintanceCommand = new Command(async () => await ExecuteEditAcquaintanceCommand()));
            }
        }

        async Task ExecuteEditAcquaintanceCommand()
        {
            await PushAsync(new AcquaintanceEditPage() { BindingContext = this });
        }


        Command _DeleteAcquaintanceCommand;

        public Command DeleteAcquaintanceCommand => _DeleteAcquaintanceCommand ??
                                                    (_DeleteAcquaintanceCommand = new Command(ExecuteDeleteAcquaintanceCommand));

        void ExecuteDeleteAcquaintanceCommand()
        {
            MessagingService.Current.SendMessage<MessagingServiceQuestion>(MessageKeys.DisplayQuestion, new MessagingServiceQuestion()
                {
                    Title = string.Format("Delete {0}?", Acquaintance.DisplayName),
                    Question = null,
                    Positive = "Delete",
                    Negative = "Cancel",
                    OnCompleted = new Action<bool>(async result =>
                        {
                            if (!result) return;

                            // pop the navigation stack twice so we get back to the list
                            await PopAsync(false);
                            await PopAsync();

                            // send a message that we want the given acquaintance to be deleted
                            MessagingService.Current.SendMessage<Acquaintance>(MessageKeys.DeleteAcquaintance, Acquaintance);
                        })
                });
        }

        Command _DialNumberCommand;

        public Command DialNumberCommand => _DialNumberCommand ??
                                            (_DialNumberCommand = new Command(ExecuteDialNumberCommand));

        void ExecuteDialNumberCommand()
        {
            if (string.IsNullOrWhiteSpace(Acquaintance.Phone))
                return;

            if (CapabilityService.CanMakeCalls)
            {
                var phoneCallTask = MessagingPlugin.PhoneDialer;
                if (phoneCallTask.CanMakePhoneCall)
                    phoneCallTask.MakePhoneCall(Acquaintance.Phone.SanitizePhoneNumber());
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

        public Command MessageNumberCommand => _MessageNumberCommand ??
                                               (_MessageNumberCommand = new Command(ExecuteMessageNumberCommand));

        void ExecuteMessageNumberCommand()
        {
            if (string.IsNullOrWhiteSpace(Acquaintance.Phone))
                return;

            if (CapabilityService.CanSendMessages)
            {
                var messageTask = MessagingPlugin.SmsMessenger;
                if (messageTask.CanSendSms)
                    messageTask.SendSms(Acquaintance.Phone.SanitizePhoneNumber());
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

        public Command EmailCommand => _EmailCommand ??
                                       (_EmailCommand = new Command(ExecuteEmailCommandCommand));

        void ExecuteEmailCommandCommand()
        {
            if (string.IsNullOrWhiteSpace(Acquaintance.Email))
                return;

            if (CapabilityService.CanSendEmail)
            {
                var emailTask = MessagingPlugin.EmailMessenger;
                if (emailTask.CanSendEmail)
                    emailTask.SendEmail(Acquaintance.Email);
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

        Command _GetDirectionsCommand;

        public Command GetDirectionsCommand
        {
            get
            {
                return _GetDirectionsCommand ??
                (_GetDirectionsCommand = new Command(async() => 
                        await ExecuteGetDirectionsCommand()));
            }
        }

        public bool IsNewAcquaintance
        {
            get
            {
                return _IsNewAcquaintance;
            }

            set
            {
                _IsNewAcquaintance = value;
            }
        }

        public ICapabilityService CapabilityService => _CapabilityService;

        async Task ExecuteGetDirectionsCommand()
        {
            var position = await GetPosition();

            var pin = new Pin() { Position = position };

            await CrossExternalMaps.Current.NavigateTo(pin.Label, pin.Position.Latitude, pin.Position.Longitude, NavigationType.Driving);
        }

        public void SetupMap()
        {
            if (HasAddress)
            {
                MessagingService.Current.SendMessage(MessageKeys.SetupMap);
            }
        }

        public void DisplayGeocodingError()
        {
            MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.DisplayAlert, new MessagingServiceAlert()
                {
                    Title = "Geocoding Error", 
                    Message = "Please make sure the address is valid, or that you have a network connection.",
                    Cancel = "OK"
                });

            IsBusy = false;
        }

        public async Task<Xamarin.Forms.Maps.Position> GetPosition()
        {
            if (!HasAddress)
                return new Xamarin.Forms.Maps.Position(0, 0);

            IsBusy = true;

            Xamarin.Forms.Maps.Position p;

            p = (await _Geocoder.GetPositionsForAddressAsync(Acquaintance.AddressString)).FirstOrDefault();

            // The Android geocoder (the underlying implementation in Android itself) fails with some addresses unless they're rounded to the hundreds.
            // So, this deals with that edge case.
            if (p.Latitude == 0 && p.Longitude == 0 && AddressBeginsWithNumber(Acquaintance.AddressString) && Device.OS == TargetPlatform.Android)
            {
                var roundedAddress = GetAddressWithRoundedStreetNumber(Acquaintance.AddressString);

                p = (await _Geocoder.GetPositionsForAddressAsync(roundedAddress)).FirstOrDefault();
            }

            IsBusy = false;

            return p;
        }
			
        void SubscribeToSaveAcquaintanceMessages()
        {
            // This subscribes to the "SaveAcquaintance" message
            MessagingService.Current.Subscribe<Acquaintance>(MessageKeys.SaveAcquaintance, (service, acquaintance) =>
                {
                    Acquaintance = acquaintance;

                    // address has been updated, so we should update the map
                    if (Acquaintance.AddressString != _AddressString)
                    {
                        MessagingService.Current.SendMessage<Acquaintance>(MessageKeys.AcquaintanceLocationUpdated, Acquaintance);

                        _AddressString = Acquaintance.AddressString;
                    }
                });
        }
			
        void SubscribeToAcquaintanceLocationUpdatedMessages()
        {
            // update the map when receiving the AcquaintanceLocationUpdated message
            MessagingService.Current.Subscribe<Acquaintance>(MessageKeys.AcquaintanceLocationUpdated, (service, acquaintance) =>
                {
                    OnPropertyChanged("HasAddress");

                    SetupMap();
                });
        }

        static bool AddressBeginsWithNumber(string address)
        {
            return !string.IsNullOrWhiteSpace(address) && char.IsDigit(address.ToCharArray().First());
        }

        static string GetAddressWithRoundedStreetNumber(string address)
        {
            var endingIndex = GetEndingIndexOfNumericPortionOfAddress(address);

            if (endingIndex == 0)
                return address;

            int originalNumber = 0;
            int roundedNumber = 0;

            int.TryParse(address.Substring(0, endingIndex + 1), out originalNumber);

            if (originalNumber == 0)
                return address;

            roundedNumber = originalNumber.RoundToLowestHundreds();

            return address.Replace(originalNumber.ToString(), roundedNumber.ToString());
        }

        static int GetEndingIndexOfNumericPortionOfAddress(string address)
        {
            int endingIndex = 0;

            for (int i = 0; i < address.Length; i++)
            {
                if (char.IsDigit(address[i]))
                    endingIndex = i;
                else
                    break;
            }

            return endingIndex;
        }
    }
}

