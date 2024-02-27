using Orleans;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Scavenger.Server.Grains
{
    public class LobbyGrain : Grain, ILobbyGrain
    {
        private Lobby _lobby;
        private HashSet<ILobbyObserver> _observers;

        public override Task OnActivateAsync(CancellationToken cancellationToken)
        {
            _lobby = new Lobby();
            _lobby.OnReady += Lobby_OnReady;

            this._observers = new HashSet<ILobbyObserver>();
            return base.OnActivateAsync(cancellationToken);
        }

        public Task GuideJoin(ILobbyObserver lobbyObserver)
        {
            Subscribe(lobbyObserver);

            var guideGrain = GrainFactory.GetGrain<IGuideGrain>(Guid.NewGuid());
            _lobby.AddGuide(guideGrain.GetPrimaryKey());

            Console.WriteLine($"Guide {_lobby.GuideId} joined Lobby {this.GetPrimaryKey()}");

            if (_lobby.IsWaitingForScavenger)
            {
                var lobbyManagerGrain = GrainFactory.GetGrain<ILobbyManagerGrain>(0);
                lobbyManagerGrain.AddLobbyWaitingForScavenger(this.GetPrimaryKey());
            }

            return Task.CompletedTask;
        }

        private void Lobby_OnReady(Lobby lobby)
        {
            var guideGrain = GrainFactory.GetGrain<IGuideGrain>(lobby.GuideId.Value);
            guideGrain.SetScavenger(lobby.ScavengerId.Value);

            var lobbyManagerGrain = GrainFactory.GetGrain<ILobbyManagerGrain>(0);

            lobbyManagerGrain.RemoveLobby(this.GetPrimaryKey());

            Task.WhenAll(_observers.Select(o => o.LobbyReady(lobby.ScavengerId.Value, lobby.GuideId.Value)));

            Console.WriteLine($"Lobby {this.GetPrimaryKey()} Ready!");
        }

        public Task ScavengerJoin(ILobbyObserver lobbyObserver)
        {
            Subscribe(lobbyObserver);

            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(Guid.NewGuid());
            _lobby.AddScavenger(scavengerGrain.GetPrimaryKey());

            Console.WriteLine($"Scavenger {_lobby.ScavengerId} joined Lobby {this.GetPrimaryKey()}");

            if (_lobby.IsWaitingForGuide)
            {
                var lobbyManagerGrain = GrainFactory.GetGrain<ILobbyManagerGrain>(0);
                lobbyManagerGrain.AddLobbyWaitingForGuide(this.GetPrimaryKey());
            }

            return Task.CompletedTask;
        }

        public Task Subscribe(ILobbyObserver observer)
        {
            this._observers.Add(observer);
            return Task.CompletedTask;
        }
        public Task Unsubscribe(ILobbyObserver observer)
        {
            this._observers.Remove(observer);
            return Task.CompletedTask;
        }
    }
}
