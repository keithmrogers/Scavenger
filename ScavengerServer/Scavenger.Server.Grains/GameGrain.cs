using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Grains
{
    public class GameGrain : Grain, IGameGrain
    {
        private Game game;
        private IAsyncStream<IDomainEvent> gameStream;
        private ICollisionChecker collisionChecker;

        public override async Task OnActivateAsync(CancellationToken cancellationToken)
        {
            collisionChecker = ServiceProvider.GetRequiredService<ICollisionChecker>();
            var gameSettings = ServiceProvider.GetRequiredService<GameSettings>();
            game = new Game(gameSettings);

            var guid = this.GetPrimaryKey();

            // Get one of the providers which we defined in config
            var streamProvider = this.GetStreamProvider("SMSProvider");

            var gameStreamId = StreamId.Create("Game", guid);
            gameStream = streamProvider.GetStream<IDomainEvent>(gameStreamId);
            
            var scavengerStreamId = StreamId.Create("Scavenger", guid);
            var scavengerStream = streamProvider.GetStream<IDomainEvent>(scavengerStreamId);

            await scavengerStream.SubscribeAsync(OnNextAsync);

            await base.OnActivateAsync(cancellationToken);
        }

        private async Task CheckFoundEgg()
        {
            var scavenger = GrainFactory.GetGrain<IScavengerGrain>(game.ScavengerId);
            game.CheckFoundEgg(await scavenger.GetScavenger(), collisionChecker);

            await StreamEvents(game);
        }

        private async Task StreamEvents(Entity entity)
        {
            await gameStream.OnNextBatchAsync(entity.DomainEvents);
        }

        public async Task OnNextAsync(IDomainEvent item, StreamSequenceToken token = null)
        {
            if (item is ScavengerPositionChangedEvent)
            {
                await CheckFoundEgg();
            }
        }

        public Task OnCompletedAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnErrorAsync(Exception ex)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> GetScavengerId()
        {
            return await Task.FromResult(game.ScavengerId);
        }

        public async Task Start(Guid scavengerId)
        {
            game.Start(scavengerId);
            await Task.CompletedTask;
        }
    }
}