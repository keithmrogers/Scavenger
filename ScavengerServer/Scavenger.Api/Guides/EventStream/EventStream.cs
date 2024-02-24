using FastEndpoints;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System.Threading.Channels;

namespace Scavenger.Api.Guides.EventStream
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

        private Dictionary<string, Channel<object>> CreateChannels()
        {
            return new Dictionary<string, Channel<object>>
            {
                { EventType.ScavengerMoved, Channel.CreateUnbounded<object>() },
                { EventType.ScavengerChangedDirection, Channel.CreateUnbounded<object>() },
                { EventType.EggFound, Channel.CreateUnbounded<object>() }
            };
        }

        public override void Configure()
        {
            Get("/api/guide/{GuideId}/event-stream");
            AllowAnonymous();
        }

        public override async Task HandleAsync(EventStreamRequest req, CancellationToken ct)
        {
            var guide = client.GetGrain<IGuideGrain>(req.GuideId);
            var gameId = await guide.GetGameId();
            var game = client.GetGrain<IGameGrain>(gameId);
            var scavengerId = await game.GetScavengerId();
            
            var streamProvider = client.GetStreamProvider("SMSProvider");

            var scavengerStream = streamProvider.GetStream<IDomainEvent>(StreamId.Create("Scavenger", scavengerId));
            await scavengerStream.SubscribeAsync(OnNextAsync);
            
            var gameStream = streamProvider.GetStream<IDomainEvent>(StreamId.Create("Game", gameId));
            await gameStream.SubscribeAsync(OnNextAsync);

            await Task.WhenAll(eventChannels.Select(kvp => SendEventStreamAsync(kvp.Key, kvp.Value.Reader.ReadAllAsync(ct), ct)));
        }

        public async Task ScavengerMoved(Position position)
        {
            await WriteEventChannelAsync(EventType.ScavengerMoved, new ScavengerMovedResponse
            {
                Position = position
            });
        }

        public async Task ScavengerChangedDirection(double direction)
        {
            await WriteEventChannelAsync(EventType.ScavengerChangedDirection, new ScavengerChangedDirectionResponse
            {
                Direction = direction
            });
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
                case ScavengerPositionChangedEvent e:
                    await ScavengerMoved(e.Position);
                    break;
                case ScavengerDirectionChangedEvent e:
                    await ScavengerChangedDirection(e.Direction);
                    break;
                case EggFoundEvent:
                    await EggFound();
                    break;
            };
        }
    }
}
