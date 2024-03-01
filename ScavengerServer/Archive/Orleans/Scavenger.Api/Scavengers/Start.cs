using FastEndpoints;
using Orleans;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Api.Scavengers
{
    public class Start
  : EndpointWithoutRequest, ILobbyObserver
    {
        private readonly IClusterClient client;
        private StartResponse? response;

        public Start(IClusterClient client)
        {
            this.client = client;
        }

        public override void Configure()
        {
            Get("/api/scavenger/start");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var lobbyObserver = client.CreateObjectReference<ILobbyObserver>(this);
            var lobbyManagerGrain = client.GetGrain<ILobbyManagerGrain>(0);

            await lobbyManagerGrain.ScavengerJoinLobby(lobbyObserver);

            while (!ct.IsCancellationRequested)//wait for the response
            {
                if (response != null)
                {
                    await SendAsync(response);
                    break;
                }
            }
        }

        public Task LobbyReady(Guid scavengerId, Guid guideId)
        {
            response = new StartResponse { ScavengerId = scavengerId };
            return Task.CompletedTask;
        }
    }
}
