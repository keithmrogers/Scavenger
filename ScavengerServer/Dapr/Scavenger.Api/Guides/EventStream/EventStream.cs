using Dapr.Actors.Client;
using Dapr.Client;
using FastEndpoints;
using Scavenger.Core;
using Scavenger.Interfaces;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Scavenger.Api.Guides.EventStream;

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
        Get("/api/guide/{GuideId}/event-stream");
        AllowAnonymous();
    }

    public override async Task HandleAsync(EventStreamRequest req, CancellationToken ct)
    {
        var reader = channelManager.GetReader(req.GuideId);

        try
        {
            await Task.WhenAll(
            [
                SendEventStreamAsync<ScavengerPositionChangedEvent, ScavengerMovedResponse>(reader, OnScavengerPositionChanged, ct),
                SendEventStreamAsync<ScavengerDirectionChangedEvent, ScavengerChangedDirectionResponse>(reader, OnScavengerDirectionChanged, ct),
                SendEventStreamAsync<EggFoundEvent, EggFoundResponse>(reader, (e) => OnEggFound(), ct),
            ]);
        }
        finally
        {
            channelManager.RemoveChannel(req.GuideId);
        }
    }

    private EggFoundResponse OnEggFound()
    {
        return new EggFoundResponse();
    }

    private ScavengerChangedDirectionResponse OnScavengerDirectionChanged(ScavengerDirectionChangedEvent @event)
    {
        return new ScavengerChangedDirectionResponse { Direction = @event.Direction };
    }

    private static ScavengerMovedResponse OnScavengerPositionChanged(ScavengerPositionChangedEvent @event)
    {
        return new ScavengerMovedResponse { Position = @event.Position };
    }

    private async Task SendEventStreamAsync<TEvent, TResponse>(ChannelReader<IDomainEvent> reader, Func<TEvent, TResponse> map, CancellationToken ct)
        where TEvent : IDomainEvent
        where TResponse : class
    {
        await SendEventStreamAsync(typeof(TEvent).Name, FilterEventsAsync(reader, map, ct), ct);
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
