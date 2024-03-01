using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;


namespace Scavenger.Server.Forms.TestClient
{
    public interface IScavengerService
    {
        void Move(Guid scavengerId, Position position);
        void ChangeDirection(Guid scavengerId, double direction);
        void Start(IScavengerClient scavengerClient);
    }

    public class ScavengerService : IScavengerService
    {
        private readonly string _ipAddress;
        private const int Port = 11000;
        private readonly ClientTerminal _clientTerminal;
        private readonly ScavengerMessageHandler _messageHandler;
        private IScavengerClient _scavengerClient;

        public ScavengerService(string ipAddress)
        {
            _ipAddress = ipAddress;
            _clientTerminal = new ClientTerminal();
            
            _clientTerminal.MessageRecived += client_MessageRecived; ;

            _messageHandler = new ScavengerMessageHandler();
            AddHandlers();
        }

        private void AddHandlers()
        {
            _messageHandler.AddHandler(ScavengerClientMessageType.LobbyReady, reader => (object)null, (client, message, data) => client.LobbyReady(message.ClientId));
        }

        private void client_MessageRecived(Socket socket, byte[] data)
        {
            _messageHandler.HandleRequest(_scavengerClient, socket, data);
        }
        
        public void Start(IScavengerClient scavengerClient)
        {
            _scavengerClient = scavengerClient;
            _clientTerminal.Connect(IPAddress.Parse(_ipAddress), Port);
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
            var message = new MessageWrapper(ClientType.Scavenger, (int)messageType, scavengerId);
            _clientTerminal.SendMessage(message.WriteBuffer(writeData));
        }
    }
}
