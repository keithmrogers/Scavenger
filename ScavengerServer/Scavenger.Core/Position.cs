using System.ComponentModel;
using System.Runtime.Serialization;

namespace Scavenger.Core;

[DataContract]
public class Position(double x, double y)
{
    [DataMember]
    public double X { get; set; } = x;

    [DataMember]
    public double Y { get; set; } = y;

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
