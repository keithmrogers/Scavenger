using Dapr.Actors;

namespace Scavenger.Interfaces;

public interface IGuideActor : IActor
{
    Task SetGameId(Guid gameId);
    Task<Guid> GetGameId();
}
