using System;

namespace Scavenger.Server.Test.TestClient
{
    public interface IScavengerClient
    {
        void LobbyReady(Guid clientId);
    }
    public class TestScavengerClient : IScavengerClient
    {
        public event Action<Guid> OnLobbyReady;

        public void LobbyReady(Guid scavengerId)
        {
            OnLobbyReady?.Invoke(scavengerId);
        }
    }
}
