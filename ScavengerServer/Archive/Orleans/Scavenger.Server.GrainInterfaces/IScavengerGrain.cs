using Orleans;
using Scavenger.Server.Domain;
using System;
using System.Threading.Tasks;

namespace Scavenger.Server.GrainInterfaces
{
    public interface IScavengerGrain : IGrainWithGuidKey
    {
        Task Move(Position position);
        Task ChangeDirection(double direction);
        Task<Domain.Scavenger> GetScavenger();
        Task<Guid> GetGameId();
    }
}
