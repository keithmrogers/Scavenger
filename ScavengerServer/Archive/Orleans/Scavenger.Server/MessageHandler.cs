using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;
using Scavenger.Server.Services;

namespace Scavenger.Server
{
    public class MessageHandler
    {
        private int bufferLength = 1000;
        private readonly IDictionary<ClientType, IDictionary<int, Func<BinaryReader, object>>> _messageReaders;
        private readonly IDictionary<int, Func<IScavengerService, MessageWrapper,Task>> _scavengerHandlers;
        private IDictionary<int, Func<IGuideService, MessageWrapper, Task>> _guideHandlers;

        public MessageHandler()
        {
            _messageReaders = new Dictionary<ClientType, IDictionary<int, Func<BinaryReader, object>>>
            {
                {ClientType.Guide, new ConcurrentDictionary<int, Func<BinaryReader, object>>()},
                {ClientType.Scavenger, new ConcurrentDictionary<int, Func<BinaryReader, object>>()}
            };
            _scavengerHandlers = new ConcurrentDictionary<int, Func<IScavengerService, MessageWrapper,Task>>();
            _guideHandlers = new ConcurrentDictionary<int, Func<IGuideService, MessageWrapper, Task>>();
        }

        public void AddScavengerHandler<T>(ScavengerServiceMessageType messageType, Func<BinaryReader, T> messageReader, Func<IScavengerService, MessageWrapper, T,Task> messageHandler) where T : class
        {
            _messageReaders[ClientType.Scavenger].Add((int)messageType, messageReader);
            _scavengerHandlers.Add((int)messageType, (service, message) => messageHandler(service, message, message.GetData<T>()));
        }

        public void AddGuideHandler<T>(GuideServiceMessageType messageType, Func<BinaryReader, T> messageReader, Func<IGuideService, MessageWrapper, T, Task> messageHandler) where T : class
        {
            _messageReaders[ClientType.Guide].Add((int)messageType, messageReader);
            _guideHandlers.Add((int)messageType, (service, message) => messageHandler(service, message, message.GetData<T>()));
        }

        //public void HandleRequest(Socket clientSocket, byte[] buffer)
        //{
        //    var messageWrapper = MessageWrapper.Load(buffer, _messageReaders);

        //    switch (messageWrapper.ClientType)
        //    {
        //        case ClientType.Scavenger:
        //            var scavengerService = new ScavengerService(new ScavengerClient(clientSocket));
        //            _scavengerHandlers[messageWrapper.MessageType](scavengerService, messageWrapper);
        //            break;
        //        case ClientType.Guide:
        //            var guideService = new GuideService(new GuideClient(clientSocket));
        //            _guideHandlers[messageWrapper.MessageType](guideService, messageWrapper);
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }
        //}

        internal async Task HandleClient(TcpClient client, CancellationToken cancellationToken)
        {
            using (client)
            {
                var buffer = new byte[bufferLength];
                var stream = client.GetStream();
                while (!cancellationToken.IsCancellationRequested)
                {
                    var amountReadTask = stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    var amountRead = amountReadTask.Result;
                    if (amountRead == 0) break;

                    var messageWrapper = MessageWrapper.Load(buffer, _messageReaders);

                    switch (messageWrapper.ClientType)
                    {
                        case ClientType.Scavenger:
                            var scavengerService = new ScavengerService();
                            await scavengerService.SetClient(new ScavengerClient(stream, messageWrapper.ClientId));
                            await _scavengerHandlers[messageWrapper.MessageType](scavengerService, messageWrapper);
                            break;
                        case ClientType.Guide:
                            var guideService = new GuideService();
                            await guideService.SetClient(new GuideClient(stream, messageWrapper.ClientId));
                            await _guideHandlers[messageWrapper.MessageType](guideService, messageWrapper);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
