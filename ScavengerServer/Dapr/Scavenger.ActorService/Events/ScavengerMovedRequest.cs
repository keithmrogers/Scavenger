using FastEndpoints;
using Scavenger.Core;

namespace Scavenger.ActorService.Events
{
    public class ScavengerMovedRequest
    {
        [FromBody]
        public ScavengerPositionChangedEvent? ScavengerPositionChangedEvent { get; set; }
    }
}