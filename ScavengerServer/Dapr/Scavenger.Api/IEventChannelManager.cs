using Scavenger.Core;
using System.Threading.Channels;

namespace Scavenger.Api
{
    public interface IEventChannelManager
    {
        ChannelReader<IDomainEvent> GetReader(Guid channelId);
        bool TryGetWriter(Guid channelId, out ChannelWriter<IDomainEvent>? writer);
        void RemoveChannel(Guid channelId);
    }
}