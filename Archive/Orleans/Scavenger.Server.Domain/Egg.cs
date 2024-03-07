using Scavenger.Server.Domain;

namespace Scavenger.Server.Domain
{
public class Egg
{
    public Egg(Position position)
    {
        this.Position = position;
    }

    public Position Position { get; set; }
}
}