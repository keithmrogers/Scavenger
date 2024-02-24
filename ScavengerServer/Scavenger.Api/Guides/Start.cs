using FastEndpoints;
using Orleans.Runtime;
using Orleans.Streams;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Api.Guides
{
    public class Start(IClusterClient client)
    : EndpointWithoutRequest<StartResponse>
    {
        private readonly IClusterClient client = client;
        private StartResponse? response;

        public override void Configure()
        {
            Get("/api/guide/start");
            AllowAnonymous();
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var lobbyManagerGrain = client.GetGrain<ILobbyManagerGrain>(0);
            var lobby = await lobbyManagerGrain.GuideJoinLobby();

            if (lobby.IsReady)
            {
                response = new StartResponse { GuideId = lobby.GuideId!.Value };
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
            response = new StartResponse { GuideId = e.GuideId };
            return Task.CompletedTask;
        }
    }
}
