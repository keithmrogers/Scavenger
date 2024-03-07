using Orleans;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IGuideGrain : IGrainWithGuidKey
    {
        Task SetGameId(Guid gameId);
        Task<Guid> GetGameId();
    }
}
