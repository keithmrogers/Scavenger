using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Orleans;
using Scavenger.Server.Clients;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Services
{
    public class GuideService : IGuideService, IDisposable
    {
        private ILobbyObserver _lobbyObserver;
        private IGuideObserver _guideObserver;
        private GuideClient _client;

        public async Task SetClient(GuideClient client)
        {
            _client = client;
            _guideObserver = await GrainClient.GrainFactory.CreateObjectReference<IGuideObserver>(client);
            _lobbyObserver = await GrainClient.GrainFactory.CreateObjectReference<ILobbyObserver>(client);

            if (client.GuideId.HasValue)
            {
                var guideGrain = GrainClient.GrainFactory.GetGrain<IGuideGrain>(client.GuideId.Value);
                await guideGrain.Subscribe(_guideObserver);
            }
        }

        public async Task ScavengerFoundEgg(Guid guideId)
        {
            var grain = GrainClient.GrainFactory.GetGrain<IGuideGrain>(guideId);
            await grain.ScavengerFoundEgg();
        }

        public async Task Start()
        {
            var lobbyManagerGrain = GrainClient.GrainFactory.GetGrain<ILobbyManagerGrain>(0);

            await lobbyManagerGrain.GuideJoinLobby(_lobbyObserver, _guideObserver);
        }

        public void Dispose()
        {
            if (_client.GuideId.HasValue)
            {
                var guideGrain = GrainClient.GrainFactory.GetGrain<IGuideGrain>(_client.GuideId.Value);
                guideGrain.Unsubscribe(_guideObserver).Wait();
            }
        }
    }
}
