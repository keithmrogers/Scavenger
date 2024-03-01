namespace Scavenger.Server.Domain
{
    public class ScavengerDirectionChangedEvent : IDomainEvent
    {
        public ScavengerDirectionChangedEvent(double direction)
        {
            Direction = direction;
        }

        public double Direction { get; }
    }
}
