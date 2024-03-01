using FastEndpoints;
using Scavenger.Core;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Scavenger.Api.Scavengers.EventStream
{
    public class EventStream
  : Endpoint<EventStreamRequest>
    {
        private readonly IEventChannelManager channelManager;

        public EventStream(IEventChannelManager channelManager)
        {
            this.channelManager = channelManager;
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
            var reader = channelManager.GetReader(req.ScavengerId);

            try
            {
                await Task.WhenAll(
                [
                    SendEventStreamAsync<EggFoundEvent, EggFoundResponse>(reader, OnEggFound, ct),
                ]);
            }
            finally
            {
                channelManager.RemoveChannel(req.ScavengerId);
            }
        }

        public EggFoundResponse OnEggFound(EggFoundEvent @event)
        {
            return new EggFoundResponse();
        }

        private async Task SendEventStreamAsync<TEvent, TResponse>(ChannelReader<IDomainEvent> reader, Func<TEvent, TResponse> map, CancellationToken ct)
            where TEvent : IDomainEvent
            where TResponse : class
        {
            await SendEventStreamAsync(nameof(TEvent), FilterEventsAsync(reader, map, ct), ct);
        }

        private static async IAsyncEnumerable<object> FilterEventsAsync<TEvent, TResponse>(ChannelReader<IDomainEvent> reader, Func<TEvent, TResponse> map, [EnumeratorCancellation] CancellationToken ct) where TResponse : class
        {
            await foreach (var item in reader.ReadAllAsync(ct))
            {
                if (item is TEvent evt)
                {
                    yield return map(evt);
                }
            }
        }
    }
}
