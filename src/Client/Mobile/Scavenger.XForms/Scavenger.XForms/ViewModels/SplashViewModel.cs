using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MvvmHelpers;
using Scavenger.Data;
using Scavenger.XForms.Pages;
using Xamarin.Forms;

namespace Scavenger.XForms.ViewModels
{
    public class SplashViewModel : BaseViewModel
    {
        private readonly IScavengerClient _scavengerClient;
        public SplashViewModel()
        {
            _scavengerClient = DependencyService.Get<IScavengerClient>();
            _scavengerClient.OnLobbyReady += Client_OnLobbyReady;
        }

        public Guid? ScavengerId;

        private Command _connectCommand;
        public Command ConnectCommand
        {
            get { return _connectCommand ?? (_connectCommand = new Command(ExecuteConnectCommand)); }
        }

        private void ExecuteConnectCommand()
        {
            ConnectCommand.ChangeCanExecute();
            Connect();
            ConnectCommand.ChangeCanExecute();
        }

        private string _waitingToConnectMessage = "Waiting for your guide to connect...";
        public string WaitingToConnectMessage
        {
            get { return _waitingToConnectMessage; }
            set
            {
                _waitingToConnectMessage = value;
                OnPropertyChanged();
            }
        }
        
        private void Connect()
        {
            var scavengerService = DependencyService.Get<IScavengerService>();
            scavengerService.Start(_scavengerClient);
            while (!ScavengerId.HasValue)
            {
                //wait for connect to complete
            }
        }

        private void Client_OnLobbyReady(Guid scavengerId)
        {
            ScavengerId = scavengerId;
        }
    }
}
