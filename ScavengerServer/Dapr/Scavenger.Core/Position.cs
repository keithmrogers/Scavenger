using System.ComponentModel;

namespace Scavenger.Core;

public class Position
{
    public Position(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Position);
    }

    private bool Equals(Position? other)
    {
        return Y.Equals(other?.Y) && X.Equals(other?.X);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return Y.GetHashCode() * 397 ^ X.GetHashCode();
        }
    }
}
