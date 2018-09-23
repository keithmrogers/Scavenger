using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;
using Scavenger.Server.Services;

namespace Scavenger.Server
{
    [Serializable]
    public class MessageWrapper
    {
        public static MessageWrapper Load(byte[] buffer, IDictionary<ClientType, IDictionary<int, Func<BinaryReader, object>>> messageReaders)
        {
            return new MessageWrapper(buffer, messageReaders);
        }

        public MessageWrapper(ClientType clientType, int messageType, Guid? clientId)
        {
            ClientType = clientType;
            MessageType = messageType;
            ClientId = clientId;
        }

        private MessageWrapper(byte[] buffer, IDictionary<ClientType, IDictionary<int, Func<BinaryReader, object>>> messageReaders)
        {
            ReadBuffer(buffer, messageReaders);
        }

        public ClientType ClientType { get; private set; }
        public int MessageType { get; private set; }
        public Guid? ClientId { get; private set; }
        private object _data;

        public byte[] WriteBuffer(Action<BinaryWriter> writeData)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(MessageType);
                    writer.Write(ClientId.GetValueOrDefault().ToString());
                    writeData?.Invoke(writer);
                }

                return ms.ToArray();
            }
        }

        private void ReadBuffer(byte[] buffer, IDictionary<ClientType, IDictionary<int, Func<BinaryReader, object>>> messageReaders)
        {
            using (var memStream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(memStream))
                {
                    ClientType = (ClientType)reader.ReadInt32();
                    MessageType = reader.ReadInt32();

                    Guid clientId;
                    if (Guid.TryParse(reader.ReadString(), out clientId))
                    {
                        ClientId = clientId;
                    }
                    else
                    {
                        throw new Exception($"Invalid ClientId for {ClientType}:{MessageType}");
                    }


                    var readMessageData = messageReaders[ClientType][MessageType];
                    
                    _data = readMessageData(reader);
                }
            }
        }

        public T GetData<T>()
        {
            return (T)_data;
        }
    }
}
