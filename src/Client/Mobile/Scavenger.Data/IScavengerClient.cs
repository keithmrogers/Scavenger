using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Data
{
    public interface IScavengerClient
    {
        event Action<Guid> OnLobbyReady;
        event Action OnEggFound;

        void LobbyReady(Guid scavengerId);
        void EggFound();
    }
}
