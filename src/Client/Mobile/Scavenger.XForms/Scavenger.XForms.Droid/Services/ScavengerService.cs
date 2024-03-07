using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Scavenger.Data;
using Scavenger.Server;
using Scavenger.XForms.Droid.Services;
using Xamarin.Forms;


[assembly: Xamarin.Forms.Dependency(typeof(ScavengerService))]
namespace Scavenger.XForms.Droid.Services
{
    public class ScavengerService : IScavengerService
    {
        private const string IpAddress = "13.67.234.244";
        //private const string IpAddress = "192.168.0.109";
        private const int Port = 11000;
        private readonly ClientTerminal _clientTerminal;
        private readonly MessageHandler _messageHandler;
        private IScavengerClient _scavengerClient;

        public ScavengerService()
        {
            _clientTerminal = new ClientTerminal();
            
            _clientTerminal.MessageRecived += client_MessageRecived;
            _clientTerminal.Connected += _clientTerminal_Connected;
            _clientTerminal.Disconnected += _clientTerminal_Disconnected;

            _messageHandler = new MessageHandler();
            AddHandlers();
        }

        private void _clientTerminal_Disconnected(Socket obj)
        {
            OnDisconnected?.Invoke();
        }

        private void _clientTerminal_Connected(Socket obj)
        {
            OnConnected?.Invoke();
        }
        

        private void AddHandlers()
        {
            _messageHandler.AddHandler(ScavengerClientMessageType.LobbyReady, reader => (object)null, (client, message, data) => client.LobbyReady(message.ClientId));
            _messageHandler.AddHandler(ScavengerClientMessageType.EggFound, reader => (object)null, (client, message, data) => client.EggFound());
        }

        private void client_MessageRecived(Socket socket, byte[] data)
        {
            _messageHandler.HandleRequest(_scavengerClient, socket, data);
        }
        
        public void Start(IScavengerClient scavengerClient)
        {
            _scavengerClient = scavengerClient;
            _clientTerminal.Connect(IPAddress.Parse(IpAddress), Port);
            _clientTerminal.StartListening();

            SendData(ScavengerServiceMessageType.Start, Guid.Empty);
        }

        public void Move(Guid scavengerId, Position position)
        {
            SendData(ScavengerServiceMessageType.Move, scavengerId, writer =>
            {
                writer.Write(position.X);
                writer.Write(position.Y);
            });
        }

        public void ChangeDirection(Guid scavengerId, double direction)
        {
            SendData(ScavengerServiceMessageType.ChangeDirection, scavengerId, writer =>
            {
                writer.Write(direction);
            });
        }

        private void SendData(ScavengerServiceMessageType messageType, Guid scavengerId, Action<BinaryWriter> writeData = null)
        {
            var message = new MessageWrapper((int)messageType, scavengerId);
            _clientTerminal.SendMessage(message.WriteBuffer(writeData));
        }

        public event Action OnConnected;

        public event Action OnDisconnected;
    }
}
