using FastEndpoints;
using Orleans;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Web.Scavengers
{
    public class ChangeDirection
  : Endpoint<ChangeDirectionRequest>
    {
        private readonly IClusterClient client;

        public ChangeDirection(IClusterClient client)
        {
            this.client = client;
        }

        public override void Configure()
        {
            Patch("/api/scavenger/{ScavengerId}/change-direction");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ChangeDirectionRequest req, CancellationToken ct)
        {
            var guideGrain = client.GetGrain<IScavengerGrain>(req.ScavengerId);
            await guideGrain.ChangeDirection(req.Direction);
        }
    }
}
