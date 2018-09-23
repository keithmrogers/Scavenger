using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scavenger.Data
{
    public interface IScavengerService
    {
        void Start(IScavengerClient scavengerClient);
        void Move(Guid scavengerId, Position position);
        void ChangeDirection(Guid scavengerId, double direction);

        event Action OnConnected;

        event Action OnDisconnected;
    }
}
