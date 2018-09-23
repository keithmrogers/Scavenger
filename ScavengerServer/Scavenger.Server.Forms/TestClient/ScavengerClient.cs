using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Server.Forms.TestClient
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
