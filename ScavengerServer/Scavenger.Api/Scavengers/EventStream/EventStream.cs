using FastEndpoints;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System.Threading.Channels;

namespace Scavenger.Api.Scavengers.EventStream
{
    public class EventStream
  : Endpoint<EventStreamRequest>
    {
        private readonly IClusterClient client;
        private readonly Dictionary<string, Channel<object>> eventChannels;

        public EventStream(IClusterClient client)
        {
            this.client = client;
            eventChannels = CreateChannels();
        }
        public override void Configure()
        {
            Get("/api/scavenger/{ScavengerId}/event-stream");
            AllowAnonymous();
        }
        private Dictionary<string, Channel<object>> CreateChannels()
        {
            return new Dictionary<string, Channel<object>>
            {
                { EventType.EggFound, Channel.CreateUnbounded<object>() }
            };
        }

        public override async Task HandleAsync(EventStreamRequest req, CancellationToken ct)
        {
            var scavengerGrain = client.GetGrain<IScavengerGrain>(req.ScavengerId);
            var gameId = await scavengerGrain.GetGameId();

            var eventChannel = Channel.CreateBounded<object>(1);
            
            var streamProvider = client.GetStreamProvider("SMSProvider");
            
            var gameStream = streamProvider.GetStream<IDomainEvent>(StreamId.Create("Game", gameId));
            await gameStream.SubscribeAsync(OnNextAsync);

            await SendEventStreamAsync("scavenger-events", eventChannel.Reader.ReadAllAsync(ct), ct);
        }

        public async Task EggFound()
        {
            await WriteEventChannelAsync(EventType.EggFound, new EggFoundResponse());
        }

        private async Task WriteEventChannelAsync(string eventName, object item)
        {
            var channel = eventChannels[eventName];
            await channel.Writer.WriteAsync(item);
        }

        public async Task OnNextAsync(IDomainEvent item, StreamSequenceToken? token = null)
        {
            switch (item)
            {
                case EggFoundEvent:
                    await EggFound();
                    break;
            };
        }
    }
}
