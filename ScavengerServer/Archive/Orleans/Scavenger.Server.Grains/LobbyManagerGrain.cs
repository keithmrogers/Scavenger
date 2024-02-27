using Orleans;
using Scavenger.Server.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class LobbyManagerGrain : Grain, ILobbyManagerGrain
    {
        private List<Guid> _lobbiesWaitingForGuides;
        private List<Guid> _lobbiesWaitingForScavengers;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _lobbiesWaitingForGuides = new List<Guid>();
            _lobbiesWaitingForScavengers = new List<Guid>();
            return base.OnActivateAsync(cancellationToken);
        }

        public async Task AddLobbyWaitingForGuide(Guid lobbyId)
        {
            _lobbiesWaitingForGuides.Add(lobbyId);
            await Task.CompletedTask;
        }

        public async Task AddLobbyWaitingForScavenger(Guid lobbyId)
        {
            _lobbiesWaitingForScavengers.Add(lobbyId);
            await Task.CompletedTask;
        }

        public async Task RemoveLobby(Guid lobbyId)
        {
            if (_lobbiesWaitingForGuides.Contains(lobbyId))
            {
                _lobbiesWaitingForGuides.Remove(lobbyId);
            }
            if (_lobbiesWaitingForScavengers.Contains(lobbyId))
            {
                _lobbiesWaitingForScavengers.Remove(lobbyId);
            }
            await Task.CompletedTask;
        }

        public async Task GuideJoinLobby(ILobbyObserver lobbyObserver)
        {
            var lobbyId = _lobbiesWaitingForGuides.Any() ? _lobbiesWaitingForGuides.First() : Guid.NewGuid();
            var lobby = GrainFactory.GetGrain<ILobbyGrain>(lobbyId);

            await lobby.GuideJoin(lobbyObserver);
        }

        public async Task ScavengerJoinLobby(ILobbyObserver lobbyObserver)
        {
            var lobbyId = _lobbiesWaitingForScavengers.Any() ? _lobbiesWaitingForScavengers.First() : Guid.NewGuid();
            var lobby = GrainFactory.GetGrain<ILobbyGrain>(lobbyId);

            await lobby.ScavengerJoin(lobbyObserver);
        }
    }
}
