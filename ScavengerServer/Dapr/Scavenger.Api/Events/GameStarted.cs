using FastEndpoints;
using Microsoft.AspNetCore.Builder;
using Scavenger.Core;

namespace Scavenger.Api.Events
{
    public class GameStarted(IEventChannelManager channelManager)
: Endpoint<GameStartedEvent>
    {
        private readonly IEventChannelManager channelManager = channelManager;

        public override void Configure()
        {
            Post("/events/game-started");
            //Options(rb => rb.WithTopic("pubsub", "games");
            Options(rb => rb.WithTopic("pubsub", "games", nameof(GameStartedEvent),1));
        }

        public override async Task HandleAsync(GameStartedEvent req, CancellationToken ct)
        {
            Console.WriteLine($"Received GameStarted event");

            if (channelManager.TryGetWriter(req.GuideId, out var gw))
            {
                await gw!.WriteAsync(req, ct);
            }

            if (channelManager.TryGetWriter(req.ScavengerId, out var sw))
            {
                await sw!.WriteAsync(req, ct);
            }

            await SendOkAsync(ct);
        }
    }
}
