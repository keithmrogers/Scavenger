
using FastEndpoints;

namespace Scavenger.Api.Scavengers
{
    public class ChangeDirectionRequest
    {
        public Guid ScavengerId { get; set; }
        public double Direction { get; set; }
    }
}