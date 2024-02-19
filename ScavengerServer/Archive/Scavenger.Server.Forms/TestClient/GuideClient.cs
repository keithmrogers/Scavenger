using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Server.Domain;

namespace Scavenger.Server.Forms.TestClient
{
    public interface IGuideClient
    {
        void LobbyReady(Guid clientId);
        void ScavengerMoved(Position position);
        void ScavengerChangedDirection(double direction);
        void EggFound(Leaderboard data);
    }
    public class TestGuideClient : IGuideClient
    {
        public event Action<Guid> OnLobbyReady;
        public event Action<Position> OnScavengerMoved;
        public event Action<double> OnScavengerChangedDirection;
        public event Action<Leaderboard> OnEggFound;

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

        public void EggFound(Leaderboard data)
        {
            OnEggFound?.Invoke(data);
        }
    }
}
