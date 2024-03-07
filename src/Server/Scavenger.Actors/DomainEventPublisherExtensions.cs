using Dapr.Client;
using Scavenger.Core;
using static Grpc.Core.Metadata;

namespace Scavenger.Actors
{
    public static class DomainEventPublisherExtensions
    {
        public static async Task PublishDomainEvents(this DaprClient client, string topicName, Entity entity)
        {
            foreach (object domainEvent in entity.DomainEvents)
            {
                Console.WriteLine($"Publishing cloudevent.type: {domainEvent.GetType().Name}");
                await client.PublishEventAsync("scavenger-pubsub", topicName, domainEvent, metadata: new Dictionary<string, string>() { { "cloudevent.type", domainEvent.GetType().Name } });
            }
        }
    }
}
