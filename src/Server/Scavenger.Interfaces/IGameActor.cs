using Dapr.Actors;
using Scavenger.Core;

namespace Scavenger.Interfaces;

public interface IGameActor : IActor
{
    Task CheckFoundEgg();
    Task<Guid> GetScavengerId();
    Task<Guid> GetGuideId();
    Task Start(Guid scavengerId, Guid guideId);
}