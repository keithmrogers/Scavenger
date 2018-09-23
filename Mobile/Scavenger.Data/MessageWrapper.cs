using System;
using System.Collections.Generic;
using System.IO;

namespace Scavenger.Data
{
    public class MessageWrapper
    {
        public static MessageWrapper Load(byte[] buffer, IDictionary<int, Func<BinaryReader, object>> messageReaders)
        {
            return new MessageWrapper(buffer, messageReaders);
        }

        public MessageWrapper(int messageType, Guid clientId)
        {
            MessageType = messageType;
            ClientId = clientId;
        }

        private MessageWrapper(byte[] buffer, IDictionary<int, Func<BinaryReader, object>> messageReaders)
        {
            ReadBuffer(buffer, messageReaders);
        }
        
        public int MessageType { get; private set; }
        public Guid ClientId { get; private set; }
        private object _data;

        public byte[] WriteBuffer(Action<BinaryWriter> writeData)
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write(1);//Scavenger client type
                    writer.Write(MessageType);
                    writer.Write(ClientId.ToString());
                    writeData?.Invoke(writer);
                }

                return ms.ToArray();
            }
        }

        private void ReadBuffer(byte[] buffer, IDictionary<int, Func<BinaryReader, object>> messageReaders)
        {
            using (var memStream = new MemoryStream(buffer))
            {
                using (var reader = new BinaryReader(memStream))
                {
                    MessageType = reader.ReadInt32();
                    ClientId = Guid.Parse(reader.ReadString());

                    var readMessageData = messageReaders[MessageType];

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
