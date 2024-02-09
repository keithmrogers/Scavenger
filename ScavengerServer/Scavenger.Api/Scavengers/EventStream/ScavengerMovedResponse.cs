using Scavenger.Server.Domain;

namespace Scavenger.Api.Scavengers.EventStream
{
    internal class ScavengerMovedResponse
    {
        public required Position Position { get; set; }
    }
}