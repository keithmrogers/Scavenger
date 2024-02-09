using Scavenger.Server.GrainInterfaces;
using System.Threading.Channels;

namespace Scavenger.Api.Scavengers.EventStream
{
    public class ScavengerChannelWriter : IScavengerObserver
    {
        private readonly ChannelWriter<object> _channelWriter;

        public ScavengerChannelWriter(ChannelWriter<object> channelWriter)
        {
            _channelWriter = channelWriter;
        }

        public async Task EggFound()
        {
            await _channelWriter.WriteAsync(new EggFoundResponse());
        }
    }
}
