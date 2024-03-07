using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scavenger.Server.Domain;
using Scavenger.Server.GrainInterfaces;

namespace Scavenger.Server.Services
{
    public interface IScavengerService
    {
        Task Start();
        Task Move(Guid scavengerId, Position position);
        Task ChangeDirection(Guid scavengerId, double direction);
    }
}
