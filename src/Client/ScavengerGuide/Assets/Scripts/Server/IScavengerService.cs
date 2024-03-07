using System;

namespace Scavenger.Server
{
    public interface IScavengerService
    {
        void Connect(Action<Guid> onConnectComplete);
        void Move(Guid scavengerId, Position position);
        void ChangeDirection(Guid scavengerId, double direction);
    }
}
