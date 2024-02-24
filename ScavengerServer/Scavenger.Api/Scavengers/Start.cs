using FastEndpoints;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Api.Scavengers
{
    public class Start
  : EndpointWithoutRequest
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
            var lobbyManagerGrain = client.GetGrain<ILobbyManagerGrain>(0);
            var lobby = await lobbyManagerGrain.ScavengerJoinLobby();

            if (lobby.IsReady)
            {
                response = new StartResponse { ScavengerId = lobby.ScavengerId!.Value };
            }
            else
            {
                var streamProvider = client.GetStreamProvider("SMSProvider");
                var stream = streamProvider.GetStream<IDomainEvent>(StreamId.Create("Lobby", lobby.LobbyId));
                await stream.SubscribeAsync(OnNextAsync);
            }

            while (!ct.IsCancellationRequested)//wait for the response
            {
                if (response != null)
                {
                    await SendAsync(response);
                    break;
                }
            }
        }

        private async Task OnNextAsync(IDomainEvent item, StreamSequenceToken? token = null)
        {
            if (item is LobbyReadyEvent e)
            {
                await LobbyReady(e);
            }
        }

        public Task LobbyReady(LobbyReadyEvent e)
        {
            response = new StartResponse { ScavengerId = e.ScavengerId };
            return Task.CompletedTask;
        }
    }
}
