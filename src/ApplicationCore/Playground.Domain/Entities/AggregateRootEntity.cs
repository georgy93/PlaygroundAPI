namespace Playground.Domain.Entities
{
    using Abstract;
    using ValueObjects;

    public class AggregateRootEntity : Entity<int>, IAggregateRoot
    {
        public Address Address { get; private set; }
    }
}