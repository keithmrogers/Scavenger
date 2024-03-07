using System.Collections.Generic;

namespace Scavenger.Server.Domain
{
    public interface ICollisionChecker
    {
        Egg CheckCollision(Scavenger scavenger, IList<Egg> listOfEggs);
    }
}