using Scavenger.Core;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Scavenger.Api
{
    public class EventChannelManager : IEventChannelManager
    {
        private readonly ConcurrentDictionary<Guid, Channel<IDomainEvent>> clientChannels = new ConcurrentDictionary<Guid, Channel<IDomainEvent>>();

        public ChannelReader<IDomainEvent> GetReader(Guid channelId)
        {
            var channel = clientChannels.GetOrAdd(channelId, (_) => Channel.CreateUnbounded<IDomainEvent>());
            return channel.Reader;
        }

        public bool TryGetWriter(Guid channelId, out ChannelWriter<IDomainEvent>? writer)
        {
            writer = null;
            if (!clientChannels.ContainsKey(channelId)) return false;

            writer = clientChannels[channelId].Writer;
            return true;
        }

        public void RemoveChannel(Guid channelId)
        {
            clientChannels.Remove(channelId, out _);
        }
    }
}
