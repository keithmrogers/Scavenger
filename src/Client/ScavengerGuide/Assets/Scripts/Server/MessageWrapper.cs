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
                    writer.Write(2);
                    writer.Write(MessageType);
                    writer.Write(ClientId.ToString());

                    if (writeData != null)
                    {
                        writeData.Invoke(writer);
                    }
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
                    ClientId = new Guid(reader.ReadString());

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
        //public enum MessageType
        //{
        //    InfoOnlyMessage = 0,
        //    Connect = 1,
        //    Disconnect = 2,
        //    Position = 3,
        //    ConnectComplete = 4,
        //    Direction = 5,
        //    ConnectGuide = 6,
        //}

        //public class MessageWrapper
        //{
        //    public MessageType Type { get; set; }
        //    public Guid ScavengerId { get; set; }
        //    public object Data { get; set; }
        //    //public List<Syncable> ListData { get; set; }

        //    public byte[] GetDataForSend()
        //    {
        //        using (var ms = new MemoryStream())
        //        {
        //            using (var writer = new BinaryWriter(ms))
        //            {
        //                writer.Write((int)Type);
        //                writer.Write(ScavengerId.ToString());

        //                if (Type == MessageType.InfoOnlyMessage)
        //                {
        //                    var messageData = (InfoOnlyMessage)Data;
        //                    writer.Write(messageData.Message);
        //                }
        //                else if (Type == MessageType.Position)
        //                {
        //                    var positionData = (Position)Data;
        //                    writer.Write(positionData.X);
        //                    writer.Write(positionData.Y);
        //                }
        //                else if (Type == MessageType.Connect)
        //                {
        //                    Connect connectData = new Connect();
        //                    if (Data != null)
        //                    {
        //                        connectData = (Connect)Data;
        //                    }

        //                    writer.Write((int)connectData.ClientType);
        //                    writer.Write(connectData.UserName ?? string.Empty);
        //                    writer.Write(connectData.Id != null ? connectData.Id.ToString() : string.Empty);
        //                }
        //                else if (Type == MessageType.ConnectGuide)
        //                {
        //                    Connect connectData = new Connect();
        //                    if (Data != null)
        //                    {
        //                        connectData = (Connect)Data;
        //                    }

        //                    writer.Write((int)connectData.ClientType);
        //                    writer.Write(connectData.UserName ?? string.Empty);
        //                    writer.Write(connectData.Id != null ? connectData.Id.ToString() : string.Empty);
        //                }
        //                else if (Type == MessageType.ConnectComplete)
        //                {
        //                    Connect connectData = new Connect();
        //                    if (Data != null)
        //                    {
        //                        connectData = (Connect)Data;
        //                    }

        //                    writer.Write((int)connectData.ClientType);
        //                    writer.Write(connectData.UserName ?? string.Empty);
        //                    writer.Write(connectData.Id != null ? connectData.Id.ToString() : string.Empty);
        //                }
        //            }

        //            return ms.ToArray();
        //        }
        //    }

        //    public static MessageWrapper ReadMessageData(byte[] incomingData)
        //    {
        //        MessageWrapper wrapper = new MessageWrapper();
        //        using (MemoryStream memStream = new MemoryStream(incomingData))
        //        {
        //            using (BinaryReader reader = new BinaryReader(memStream))
        //            {
        //                wrapper.Type = (MessageType)reader.ReadInt32();
        //                wrapper.ScavengerId = new Guid(reader.ReadString());

        //                if (wrapper.Type == MessageType.InfoOnlyMessage)
        //                {
        //                    InfoOnlyMessage message = new InfoOnlyMessage();
        //                    message.Message = reader.ReadString();
        //                    wrapper.Data = message;
        //                }
        //                else if (wrapper.Type == MessageType.Position)
        //                {
        //                    Position position = new Position();
        //                    position.X = reader.ReadDouble();
        //                    position.Y = reader.ReadDouble();

        //                    wrapper.Data = position;
        //                }
        //                else if (wrapper.Type == MessageType.Connect)
        //                {
        //                    Connect connect = new Connect();
        //                    connect.ClientType = (ClientTypes)reader.ReadInt32();
        //                    connect.UserName = reader.ReadString();
        //                    connect.Id = new Guid(reader.ReadString());

        //                    wrapper.Data = connect;
        //                }
        //                else if (wrapper.Type == MessageType.ConnectGuide)
        //                {
        //                    Connect connect = new Connect();
        //                    connect.ClientType = (ClientTypes)reader.ReadInt32();
        //                    connect.UserName = reader.ReadString();
        //                    connect.Id = new Guid(reader.ReadString());

        //                    wrapper.Data = connect;
        //                }
        //                else if (wrapper.Type == MessageType.ConnectComplete)
        //                {
        //                    Connect connect = new Connect();
        //                    connect.ClientType = (ClientTypes)reader.ReadInt32();
        //                    connect.UserName = reader.ReadString();
        //                    connect.Id = new Guid(reader.ReadString());

        //                    wrapper.Data = connect;
        //                }
        //            }

        //            return wrapper;
        //        }
        //    }
        //}
    }