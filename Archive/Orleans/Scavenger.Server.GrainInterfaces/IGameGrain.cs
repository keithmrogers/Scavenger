using System;
using System.Threading.Tasks;
using Orleans;

namespace Scavenger.Server.GrainInterfaces;

public interface IGameGrain : IGrainWithGuidKey
{
    Task<Guid> GetScavengerId();
    Task Start(Guid scavengerId);
}