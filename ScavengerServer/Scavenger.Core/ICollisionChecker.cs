using System.Collections.Generic;

namespace Scavenger.Core
{
    public interface ICollisionChecker
    {
        Egg? CheckCollision(Scavenger scavenger, IList<Egg> listOfEggs);
    }
}