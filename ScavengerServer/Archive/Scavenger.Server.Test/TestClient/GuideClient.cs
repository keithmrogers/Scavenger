using System;
using Scavenger.Server.Domain;

namespace Scavenger.Server.Test.TestClient
{
    public interface IGuideClient
    {
        void LobbyReady(Guid clientId);
        void ScavengerMoved(Position position);
        void ScavengerChangedDirection(double direction);
    }
    public class TestGuideClient : IGuideClient
    {
        public event Action<Guid> OnLobbyReady;
        public event Action<Position> OnScavengerMoved;
        public event Action<double> OnScavengerChangedDirection;

        public void LobbyReady(Guid scavengerId)
        {
            OnLobbyReady?.Invoke(scavengerId);
        }
        public void ScavengerMoved(Position position)
        {
            OnScavengerMoved?.Invoke(position);
        }
        public void ScavengerChangedDirection(double direction)
        {
            OnScavengerChangedDirection?.Invoke(direction);
        }
    }
}
