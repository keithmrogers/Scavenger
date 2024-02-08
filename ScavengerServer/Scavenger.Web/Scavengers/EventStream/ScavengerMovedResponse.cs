using Scavenger.Server.Domain;

namespace Scavenger.Web.Scavengers.EventStream
{
    internal class ScavengerMovedResponse
    {
        public required Position Position { get; set; }
    }
}