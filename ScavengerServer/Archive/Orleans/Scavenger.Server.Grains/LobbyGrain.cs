using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class LobbyGrain : Grain, ILobbyGrain
    {
        private Lobby lobby;
        private IAsyncStream<IDomainEvent> stream;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            lobby = new Lobby(this.GetPrimaryKey());
            
            var streamProvider = this.GetStreamProvider("SMSProvider");
            stream = streamProvider.GetStream<IDomainEvent>(StreamId.Create("Lobby", this.GetPrimaryKey()));

            stream.SubscribeAsync(OnNextAsync);

            return base.OnActivateAsync(cancellationToken);
        }

        public async Task<Lobby> GuideJoin()
        {
            var guideGrain = GrainFactory.GetGrain<IGuideGrain>(Guid.NewGuid());
            lobby.AddGuide(guideGrain.GetPrimaryKey());

            await StreamEvents(lobby);

            Console.WriteLine($"Guide {lobby.GuideId} joined Lobby {this.GetPrimaryKey()}");

            return await Task.FromResult(lobby);
        }

        public async Task<Lobby> ScavengerJoin()
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(Guid.NewGuid());
            lobby.AddScavenger(scavengerGrain.GetPrimaryKey());

            await StreamEvents(lobby);
            
            Console.WriteLine($"Guide {lobby.ScavengerId} joined Lobby {this.GetPrimaryKey()}");

            return await Task.FromResult(lobby);
        }

        private async Task StreamEvents(Entity entity)
        {
            await stream.OnNextBatchAsync(entity.DomainEvents);
        }

        private async Task OnNextAsync(IDomainEvent item, StreamSequenceToken token = null)
        {
            if(item is LobbyReadyEvent e){
                await LobbyReady(e);
            }
        }

        private async Task LobbyReady(LobbyReadyEvent e)
        {
            var gameGrain = GrainFactory.GetGrain<IGameGrain>(Guid.NewGuid());

            await gameGrain.Start(e.ScavengerId);
            
            var guideGrain = GrainFactory.GetGrain<IGuideGrain>(e.GuideId);
            await guideGrain.SetGameId(gameGrain.GetPrimaryKey());

            Console.WriteLine($"Lobby {this.GetPrimaryKey()} Ready!");
        }
    }
}
