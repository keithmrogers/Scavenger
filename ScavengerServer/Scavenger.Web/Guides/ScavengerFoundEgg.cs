using FastEndpoints;
using Orleans;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Web.Guides
{
    public class ScavengerFoundEgg
  : Endpoint<ScavengerFoundEggRequest>
    {
        private readonly IClusterClient client;

        public ScavengerFoundEgg(IClusterClient client)
        {
            this.client = client;
        }

        public override void Configure()
        {
            Patch("/api/guide/scavenger-found-egg");
            AllowAnonymous();
        }

        public override async Task HandleAsync(ScavengerFoundEggRequest req, CancellationToken ct)
        {
            var guideGrain = client.GetGrain<IGuideGrain>(req.GuideId);
            await guideGrain.ScavengerFoundEgg();
        }
    }
}
