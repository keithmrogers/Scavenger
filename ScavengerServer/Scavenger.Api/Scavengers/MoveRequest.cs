
using FastEndpoints;
using Scavenger.Server.Domain;

namespace Scavenger.Api.Scavengers
{
    public class MoveRequest
    {
        public Guid ScavengerId { get; set; }
        public Position? Position { get; set; }
    }
}