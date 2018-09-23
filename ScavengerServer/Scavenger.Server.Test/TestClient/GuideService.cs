using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;

namespace Scavenger.Server.Test.TestClient
{
    public interface IGuideService
    {
        void Start(IGuideClient client);
    }
    public class GuideService : IGuideService
    {
        //private const string IpAddress = "13.67.234.244";
        private readonly string _ipAddress;
        private const int Port = 11000;
        private readonly ClientTerminal _clientTerminal;
        private readonly GuideMessageHandler _messageHandler;
        private IGuideClient _guideClient;

        public GuideService(string ipAddress)
        {
            _ipAddress = ipAddress;
            _clientTerminal = new ClientTerminal();

            _clientTerminal.MessageRecived += client_MessageRecived; ;

            _messageHandler = new GuideMessageHandler();
            AddHandlers();
        }

        private void AddHandlers()
        {
            _messageHandler.AddHandler(GuideClientMessageType.LobbyReady, reader => (object)null, (client, message, data) => client.LobbyReady(message.ClientId));
            _messageHandler.AddHandler(GuideClientMessageType.ScavengerMoved, reader => new Position(reader.ReadDouble(), reader.ReadDouble()), (client, message, data) => client.ScavengerMoved(data));
            _messageHandler.AddHandler(GuideClientMessageType.ScavengerChangedDirection, reader => new { Direction = reader.ReadDouble() }, (client, message, data) => client.ScavengerChangedDirection(data.Direction));
        }

        private void client_MessageRecived(Socket socket, byte[] data)
        {
            _messageHandler.HandleRequest(_guideClient, socket, data);
        }

        public void Start(IGuideClient guideClient)
        {
            _guideClient = guideClient;
            _clientTerminal.Connect(IPAddress.Parse(_ipAddress), Port);
            _clientTerminal.StartListening();

            SendData(GuideServiceMessageType.Start, Guid.Empty);
        }

        private void SendData(GuideServiceMessageType messageType, Guid scavengerId, Action<BinaryWriter> writeData = null)
        {
            var message = new MessageWrapper(ClientType.Guide, (int)messageType, scavengerId);
            _clientTerminal.SendMessage(message.WriteBuffer(writeData));
        }
    }
}
