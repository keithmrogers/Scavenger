using Dapr;
using Scavenger.ActorService;
using Scavenger.Core;

namespace Scavenger.ActorService
{
    public static class DomainEventTopicExtensions
    {
        public static T WithTopic<T>(this T builder, string pubsubName, string name, string eventType, int priority)
            where T : IEndpointConventionBuilder
        {
            return builder.WithTopic(new TopicOptions
            {
                PubsubName = pubsubName,
                Name = name,
                Match = $"event.type ==\"{eventType}\"",
                Priority = priority,
            });
        }
    }
}