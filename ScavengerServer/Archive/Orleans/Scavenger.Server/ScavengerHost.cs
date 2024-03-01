using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Orleans;
using Orleans.Runtime.Configuration;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;
using Scavenger.Server.Services;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Runtime;

namespace Scavenger.Server
{
    public class ScavengerHost
    {
        private readonly MessageHandler _messageHandler;
        private readonly IPEndPoint _endPoint;
        private readonly ServerTerminal _listener = new ServerTerminal();

        public ScavengerHost(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _messageHandler = new MessageHandler();
            AddGuideHandlers();
            AddScavengerHandlers();
        }

        private void AddGuideHandlers()
        {
            _messageHandler.AddGuideHandler(GuideServiceMessageType.Start, reader => (object)null, (service, wrapper, data) => service.Start());
            _messageHandler.AddGuideHandler(GuideServiceMessageType.ScavengerFoundEgg, reader => (object)null, (service, wrapper, data) => service.ScavengerFoundEgg(wrapper.ClientId.GetValueOrDefault()));
        }

        private void AddScavengerHandlers()
        {
            _messageHandler.AddScavengerHandler(ScavengerServiceMessageType.ChangeDirection, reader => new { Direction = reader.ReadDouble() }, (service, wrapper, data) => service.ChangeDirection(wrapper.ClientId.GetValueOrDefault(), data.Direction));
            _messageHandler.AddScavengerHandler(ScavengerServiceMessageType.Move, reader => new Position(reader.ReadDouble(), reader.ReadDouble()), (service, wrapper, data) => service.Move(wrapper.ClientId.GetValueOrDefault(), data));
            _messageHandler.AddScavengerHandler(ScavengerServiceMessageType.Start, reader => (object)null, (service, wrapper, data) => service.Start());
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            Console.WriteLine("Initializing Grain Client...");
            GrainClient.Initialize("OrleansClientConfig.xml");
            _listener.HandleClient += Listener_HandleClient;
            Console.WriteLine("Listening on {0}...", _endPoint);
            await _listener.StartListen(_endPoint, cancellationToken);
        }

        private async Task Listener_HandleClient(TcpClient client, System.Threading.CancellationToken cancellationToken)
        {
            await _messageHandler.HandleClient(client, cancellationToken);
        }
    }
}
