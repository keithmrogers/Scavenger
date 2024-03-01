using Dapr.Actors;
using Scavenger.Core;

namespace Scavenger.Interfaces;

public interface IScavengerActor : IActor
{
    Task Move(Position position);
    Task ChangeDirection(double direction);
    Task<Core.Scavenger> GetScavenger();
    Task Start(Guid gameId);

}
