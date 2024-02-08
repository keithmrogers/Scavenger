using FastEndpoints;
using Orleans;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Web.Scavengers
{
    public class Move
  : Endpoint<MoveRequest>
    {
        private readonly IClusterClient client;

        public Move(IClusterClient client)
        {
            this.client = client;
        }

        public override void Configure()
        {
            Patch("/api/scavenger/{ScavengerId}/move");
            AllowAnonymous();
        }

        public override async Task HandleAsync(MoveRequest req, CancellationToken ct)
        {
            var guideGrain = client.GetGrain<IScavengerGrain>(req.ScavengerId);
            await guideGrain.Move(req.Position);
        }
    }
}
