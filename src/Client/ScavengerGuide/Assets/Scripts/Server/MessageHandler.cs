using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.IO;

namespace Scavenger.Server
{
    using System.Collections.Generic;
    using System.IO;

    public enum GuideClientMessageType
    {
        LobbyReady = 1,
        ScavengerMoved = 2,
        ScavengerChangedDirection = 3
    }
    public enum GuideServiceMessageType
    {
        Start = 1,
        ScavengerFoundEgg = 2
    }

    public class MessageHandler
    {
        private readonly IDictionary<int, Func<BinaryReader, object>> _messageReaders;
        private readonly IDictionary<int, Action<IGuideClient, MessageWrapper>> _guideHandlers;

        public MessageHandler()
        {
            _messageReaders = new Dictionary<int, Func<BinaryReader, object>>();
            _guideHandlers = new Dictionary<int, Action<IGuideClient, MessageWrapper>>();
        }

        public void AddHandler<T>(GuideClientMessageType messageType, Func<BinaryReader, object> messageReader, Action<IGuideClient, MessageWrapper, object> messageHandler)
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