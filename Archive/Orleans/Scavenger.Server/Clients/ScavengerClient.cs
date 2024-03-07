using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Clients
{
    public class ScavengerClient : ILobbyObserver, IScavengerObserver
    {
        //private Socket _clientSocket;
        private readonly NetworkStream _stream;

        public Guid? ScavengerId { get; private set; }

        public ScavengerClient(NetworkStream stream, Guid? scavengerId)
        {
            _stream = stream;
            ScavengerId = scavengerId;
        }

        public void LobbyReady(Guid scavengerId, Guid guideId)
        {
            ScavengerId = scavengerId;
            SendMessage(ScavengerClientMessageType.LobbyReady);
        }

        public void EggFound()
        {
            SendMessage(ScavengerClientMessageType.EggFound);
        }

        private async void SendMessage(ScavengerClientMessageType messageType, Action<BinaryWriter> writeData = null)
        {
            if (!_stream.CanWrite) return;
            var message = new MessageWrapper(ClientType.Scavenger, (int)messageType, ScavengerId);
            var buffer = message.WriteBuffer(writeData);
            await _stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }
    }
}
