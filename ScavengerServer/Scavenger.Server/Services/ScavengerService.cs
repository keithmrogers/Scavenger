using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Orleans;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Services
{
    public class ScavengerService : IScavengerService, IDisposable
    {
        private ILobbyObserver _lobbyObserver;
        private IScavengerObserver _scavengerObserver;
        private ScavengerClient _client;

        public async Task SetClient(ScavengerClient client)
        {
            _client = client;
            _scavengerObserver = await GrainClient.GrainFactory.CreateObjectReference<IScavengerObserver>(client);
            _lobbyObserver = await GrainClient.GrainFactory.CreateObjectReference<ILobbyObserver>(client);

            if (client.ScavengerId.HasValue)
            {
                var scavengerGrain = GrainClient.GrainFactory.GetGrain<IScavengerGrain>(client.ScavengerId.Value);
                await scavengerGrain.SubscribeScavenger(_scavengerObserver);
            }
        }

        public async Task Start()
        {
            var lobbyManagerGrain = GrainClient.GrainFactory.GetGrain<ILobbyManagerGrain>(0);

            await lobbyManagerGrain.ScavengerJoinLobby(_lobbyObserver, _scavengerObserver);
        }

        public async Task Move(Guid scavengerId, Position position)
        {
            await LogPosition(position);
            var grain = GrainClient.GrainFactory.GetGrain<IScavengerGrain>(scavengerId);
            await grain.Move(position);
        }

        private async Task LogPosition(Position position)
        {
            using (var positionTracker = File.AppendText("PositionTracker.csv"))
            {
                await positionTracker.WriteLineAsync(position.X + "," + position.Y + "," + DateTime.Now.Ticks);
            }
        }

        public async Task ChangeDirection(Guid scavengerId, double direction)
        {
            await LogDirection(direction);
            var grain = GrainClient.GrainFactory.GetGrain<IScavengerGrain>(scavengerId);
            await grain.ChangeDirection(direction);
        }

        private async Task LogDirection(double direction)
        {
            using (var directionTracker = File.AppendText("DirectionTracker.csv"))
            {
                await directionTracker.WriteLineAsync(direction + "," + DateTime.Now.Ticks);
            }
        }

        public void Dispose()
        {
            if (_client.ScavengerId.HasValue)
            {
                var scavengerGrain = GrainClient.GrainFactory.GetGrain<IScavengerGrain>(_client.ScavengerId.Value);
                scavengerGrain.UnsubscribeScavenger(_scavengerObserver)
                    .Wait();
            }
        }
    }
}
