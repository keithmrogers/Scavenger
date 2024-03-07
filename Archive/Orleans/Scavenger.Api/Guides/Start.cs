using FastEndpoints;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Api.Guides
{
    public class Start
  : EndpointWithoutRequest<StartResponse>, ILobbyObserver
    {
        private readonly IClusterClient client;
        private StartResponse? response;

        public Start(IClusterClient client)
        {
            this.client = client;
        }

        public override void Configure()
        {
            Get("/api/guide/start");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var lobbyObserver = client.CreateObjectReference<ILobbyObserver>(this);
            var lobbyManagerGrain = client.GetGrain<ILobbyManagerGrain>(0);

            await lobbyManagerGrain.GuideJoinLobby(lobbyObserver);

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
            response = new StartResponse { GuideId = guideId };
            return Task.CompletedTask;
        }
    }
}
