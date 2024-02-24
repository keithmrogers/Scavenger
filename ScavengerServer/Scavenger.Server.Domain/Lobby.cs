using System;

namespace Scavenger.Server.Domain
{
    public class Lobby(Guid lobbyId) : Entity
    {
        private Guid? _scavengerId;
        private Guid? _guideId;

        public Guid? ScavengerId
        {
            get { return _scavengerId; }
        }

        public Guid? GuideId
        {
            get { return _guideId; }
        }

        public void AddScavenger(Guid scavengerId)
        {
            if (_scavengerId.HasValue)
            {
                throw new Exception("A scavenger has already joined the lobby");
            }

            _scavengerId = scavengerId;
            NotifyIfReady();
        }

        public void AddGuide(Guid guideId)
        {
            if (_guideId.HasValue)
            {
                throw new Exception("A guide has already joined the lobby");
            }

            _guideId = guideId;
            NotifyIfReady();
        }

        public bool IsReady { get { return ScavengerId.HasValue && GuideId.HasValue; } }

        public bool IsWaitingForGuide { get { return !GuideId.HasValue; } }

        public bool IsWaitingForScavenger { get { return !ScavengerId.HasValue; } }

        public Guid LobbyId { get; set; } = lobbyId;

        private void NotifyIfReady()
        {
            if (IsReady)
            {
                AddDomainEvent(new LobbyReadyEvent(ScavengerId.Value, GuideId.Value));
            }
        }
    }
}
