using FastEndpoints;
using Orleans;
using Scavenger.Server.GrainInterfaces;
using System.Threading.Channels;

namespace Scavenger.Web.Scavengers.EventStream
{
    public class EventStream
  : Endpoint<EventStreamRequest>
    {
        private readonly IClusterClient client;

        public EventStream(IClusterClient client)
        {
            this.client = client;
        }
        public override void Configure()
        {
            Get("/api/scavenger/{ScavengerId}/event-stream");
            AllowAnonymous();
        }

        public override async Task HandleAsync(EventStreamRequest req, CancellationToken ct)
        {
            var eventChannel = Channel.CreateBounded<object>(1);
            var channelWriter = new ScavengerChannelWriter(eventChannel.Writer);
            var scavengerObserver = client.CreateObjectReference<IScavengerObserver>(channelWriter);
            var scavengerGrain = client.GetGrain<IScavengerGrain>(req.ScavengerId);
            await scavengerGrain.SubscribeScavenger(scavengerObserver);

            await SendEventStreamAsync("scavenger-events", eventChannel.Reader.ReadAllAsync(ct), ct);
        }
    }
}
