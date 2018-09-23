using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Scavenger.Server.Test.TestClient
{
    public class GuideMessageHandler
    {
        private readonly IDictionary<int, Func<BinaryReader, object>> _messageReaders;
        private readonly IDictionary<int, Action<IGuideClient, MessageWrapper>> _guideHandlers;

        public GuideMessageHandler()
        {
            _messageReaders = new ConcurrentDictionary<int, Func<BinaryReader, object>>();
            _guideHandlers = new ConcurrentDictionary<int, Action<IGuideClient, MessageWrapper>>();
        }

        public void AddHandler<T>(GuideClientMessageType messageType, Func<BinaryReader, T> messageReader, Action<IGuideClient, MessageWrapper, T> messageHandler) where T : class
        {
            _messageReaders.Add((int)messageType, messageReader);
            _guideHandlers.Add((int)messageType, (service, message) => messageHandler(service, message, message.GetData<T>()));
        }

        public void HandleRequest(IGuideClient GuideClient, Socket clientSocket, byte[] buffer)
        {
            var messageWrapper = MessageWrapper.Load(buffer, _messageReaders);
            
            _guideHandlers[messageWrapper.MessageType](GuideClient, messageWrapper);
        }
    }
}
