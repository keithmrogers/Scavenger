using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Grains
{
    public class LobbyGrain : Grain, ILobbyGrain
    {
        private Lobby _lobby;

        private ObserverSubscriptionManager<ILobbyObserver> _observers;
        private IGuideObserver _guideObserver;
        private IScavengerObserver _scavengerObserver;

        public override Task OnActivateAsync()
        {
            _lobby = new Lobby();
            _lobby.OnReady += Lobby_OnReady;

            this._observers = new ObserverSubscriptionManager<ILobbyObserver>();
            return base.OnActivateAsync();
        }

        public Task GuideJoin(ILobbyObserver lobbyObserver, IGuideObserver guideObserver)
        {
            Subscribe(lobbyObserver);
            _guideObserver = guideObserver;

            var guideGrain = GrainFactory.GetGrain<IGuideGrain>(Guid.NewGuid());
            _lobby.AddGuide(guideGrain.GetPrimaryKey());

            Console.WriteLine($"Guide {_lobby.GuideId} joined Lobby { this.GetPrimaryKey()}");

            if (_lobby.IsWaitingForScavenger)
            {
                var lobbyManagerGrain = GrainFactory.GetGrain<ILobbyManagerGrain>(0);
                lobbyManagerGrain.AddLobbyWaitingForScavenger(this.GetPrimaryKey());
            }

            return TaskDone.Done;
        }

        private void Lobby_OnReady(Lobby lobby)
        {
            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(lobby.ScavengerId.Value);
            scavengerGrain.SubscribeGuide(_guideObserver);
            scavengerGrain.SubscribeScavenger(_scavengerObserver);

            var guideGrain = GrainFactory.GetGrain<IGuideGrain>(lobby.GuideId.Value);
            guideGrain.SetScavenger(lobby.ScavengerId.Value);

            var lobbyManagerGrain = GrainFactory.GetGrain<ILobbyManagerGrain>(0);

            lobbyManagerGrain.RemoveLobby(this.GetPrimaryKey());

            _observers.Notify(o => o.LobbyReady(lobby.ScavengerId.Value, lobby.GuideId.Value));

            Console.WriteLine($"Lobby { this.GetPrimaryKey()} Ready!");
        }

        public Task ScavengerJoin(ILobbyObserver lobbyObserver, IScavengerObserver scavengerObserver)
        {
            Subscribe(lobbyObserver);
            _scavengerObserver = scavengerObserver;

            var scavengerGrain = GrainFactory.GetGrain<IScavengerGrain>(Guid.NewGuid());
            _lobby.AddScavenger(scavengerGrain.GetPrimaryKey());
            
            Console.WriteLine($"Scavenger {_lobby.ScavengerId} joined Lobby { this.GetPrimaryKey()}");

            if (_lobby.IsWaitingForGuide)
            {
                var lobbyManagerGrain = GrainFactory.GetGrain<ILobbyManagerGrain>(0);
                lobbyManagerGrain.AddLobbyWaitingForGuide(this.GetPrimaryKey());
            }

            return TaskDone.Done;
        }

        public Task Subscribe(ILobbyObserver observer)
        {
            this._observers.Subscribe(observer);
            return TaskDone.Done;
        }
        public Task Unsubscribe(ILobbyObserver observer)
        {
            this._observers.Unsubscribe(observer);
            return TaskDone.Done;
        }
    }
}
