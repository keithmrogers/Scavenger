using Dapr;
using Scavenger.Core;

namespace Scavenger.Api
{
    public static class DomainEventTopicExtensions
    {
        public static T WithTopic<T>(this T builder, string pubsubName, string name, string eventType, int priority)
            where T : IEndpointConventionBuilder
        {
            Console.WriteLine($"Add Subscription {pubsubName} {name} {eventType}");
            return builder.WithTopic(new TopicOptions
            {
                PubsubName = pubsubName,
                Name = name,
                Match= $"event.type == \"{eventType}\"",
                Priority = priority,
            });
        }
    }
}