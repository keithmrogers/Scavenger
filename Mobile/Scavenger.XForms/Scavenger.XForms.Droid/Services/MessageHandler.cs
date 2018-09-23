using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using Scavenger.Data;
using Scavenger.XForms;
using Scavenger.XForms.Droid.Services;

namespace Scavenger.Server
{
    public class MessageHandler
    {
        private readonly IDictionary<int, Func<BinaryReader, object>> _messageReaders;
        private readonly IDictionary<int, Action<IScavengerClient, MessageWrapper>> _scavengerHandlers;

        public MessageHandler()
        {
            _messageReaders = new ConcurrentDictionary<int, Func<BinaryReader, object>>();
            _scavengerHandlers = new ConcurrentDictionary<int, Action<IScavengerClient, MessageWrapper>>();
        }

        public void AddHandler<T>(ScavengerClientMessageType messageType, Func<BinaryReader, T> messageReader, Action<IScavengerClient, MessageWrapper, T> messageHandler) where T : class
        {
            _messageReaders.Add((int)messageType, messageReader);
            _scavengerHandlers.Add((int)messageType, (service, message) => messageHandler(service, message, message.GetData<T>()));
        }

        public void HandleRequest(IScavengerClient scavengerClient, Socket clientSocket, byte[] buffer)
        {
            var messageWrapper = MessageWrapper.Load(buffer, _messageReaders);
            
            _scavengerHandlers[messageWrapper.MessageType](scavengerClient, messageWrapper);
        }
    }
}
