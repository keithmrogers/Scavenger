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
    public class GuideClient : ILobbyObserver, IGuideObserver
    {
        //private Socket _clientSocket;
        private readonly NetworkStream _stream;
        public Guid? GuideId { get; private set; }

        public GuideClient(NetworkStream stream, Guid? guideId)
        {
            _stream = stream;
            GuideId = guideId;
        }


        public void LobbyReady(Guid scavengerId, Guid guideId)
        {
            GuideId = guideId;
            SendMessage(GuideClientMessageType.LobbyReady);
        }

        public void ScavengerChangedDirection(double direction)
        {
            SendMessage(GuideClientMessageType.ScavengerChangedDirection, writer =>
            {
                writer.Write(direction);
            });
        }

        public void ScavengerMoved(Position position)
        {
            SendMessage(GuideClientMessageType.ScavengerMoved, writer =>
            {
                writer.Write(position.X);
                writer.Write(position.Y);
            });
        }
        
        public void EggFound(Leaderboard leaderboard)
        {
            SendMessage(GuideClientMessageType.EggFound, writer => { writer.Write(leaderboard.FastestEggFindMs); writer.Write(leaderboard.FarthestDistanceBetweenEggFindsM); writer.Write(leaderboard.ShortestTimeBetweenEggFindsMs); });
        }

        private async void SendMessage(GuideClientMessageType messageType, Action<BinaryWriter> writeData = null)
        {
            if (!_stream.CanWrite) return;
            var message = new MessageWrapper(ClientType.Guide, (int) messageType, GuideId);
            var buffer = message.WriteBuffer(writeData);
            await _stream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
        }

    }
}
