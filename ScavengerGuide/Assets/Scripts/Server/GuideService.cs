using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System;
using System.IO;

namespace Scavenger.Server
{
    public interface IGuideService
    {
        void Start(IGuideClient client);
        void FoundEgg(Guid scavengerId);
    }

    public class GuideService : IGuideService
    {
        //private const string IpAddress = "13.67.234.244";
        private const string IpAddress = "192.168.1.85";
        private const int Port = 11000;
        private readonly ClientTerminal _clientTerminal;
        private readonly MessageHandler _messageHandler;
        private IGuideClient _guideClient;

        public GuideService()
        {
            _clientTerminal = new ClientTerminal();

            _clientTerminal.MessageRecived += client_MessageRecived;

            _messageHandler = new MessageHandler();
            AddHandlers();
        }

        private void AddHandlers()
        {
            _messageHandler.AddHandler<object>(GuideClientMessageType.LobbyReady, reader => (object)null, (client, message, data) => client.LobbyReady(message.ClientId));
            _messageHandler.AddHandler<Position>(GuideClientMessageType.ScavengerMoved, reader => new Position(reader.ReadDouble(), reader.ReadDouble()), (client, message, data) => client.ScavengerMoved((Position)data));
            _messageHandler.AddHandler<double>(GuideClientMessageType.ScavengerChangedDirection, reader => reader.ReadDouble(), (client, message, data) => client.ScavengerChangedDirection((double)data));
        }

        private void client_MessageRecived(Socket socket, byte[] data)
        {
            _messageHandler.HandleRequest(_guideClient, socket, data);
        }

        public void Start(IGuideClient guideClient)
        {
            _guideClient = guideClient;
            _clientTerminal.Connect(IPAddress.Parse(IpAddress), Port);
            _clientTerminal.StartListening();

            SendData(GuideServiceMessageType.Start, Guid.Empty);
        }

        public void FoundEgg(Guid scavengerId)
        {
            SendData(GuideServiceMessageType.ScavengerFoundEgg, scavengerId);
        }

        private void SendData(GuideServiceMessageType messageType, Guid scavengerId, Action<BinaryWriter> writeData = null)
        {
            var message = new MessageWrapper((int)messageType, scavengerId);
            _clientTerminal.SendMessage(message.WriteBuffer(writeData));
        }
    }
}