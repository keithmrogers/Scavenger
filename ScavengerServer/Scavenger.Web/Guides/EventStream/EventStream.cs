using FastEndpoints;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System.Threading.Channels;

namespace Scavenger.Web.Guides.EventStream
{
    public class EventStream
  : Endpoint<EventStreamRequest>, IGuideObserver
    {
        private readonly IClusterClient client;
        private readonly Dictionary<string, Channel<object>> eventChannels;

        public EventStream(IClusterClient client)
        {
            this.client = client;
            this.eventChannels = CreateChannels();
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
            var guideObserver = client.CreateObjectReference<IGuideObserver>(this);
            var guideGrain = client.GetGrain<IGuideGrain>(req.GuideId);
            await guideGrain.Subscribe(guideObserver);

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

        public async Task EggFound(Leaderboard leaderboard)
        {
            await WriteEventChannelAsync(EventType.EggFound, new EggFoundResponse
            {
                FastestEggFindMs = leaderboard.FastestEggFindMs,
                FarthestDistanceBetweenEggFindsM = leaderboard.FarthestDistanceBetweenEggFindsM,
                ShortestTimeBetweenEggFindsMs = leaderboard.ShortestTimeBetweenEggFindsMs
            });
        }

        private async Task WriteEventChannelAsync(string eventName, object item)
        {
            var channel = eventChannels[eventName];
            await channel.Writer.WriteAsync(item);
        }
    }
}
