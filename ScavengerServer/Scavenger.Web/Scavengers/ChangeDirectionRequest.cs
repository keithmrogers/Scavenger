
using FastEndpoints;

namespace Scavenger.Web.Scavengers
{
    public class ChangeDirectionRequest
    {
        public Guid ScavengerId { get; set; }
        public double Direction { get; set; }
    }
}