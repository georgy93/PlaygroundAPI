namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using System;

    public record UserId
    {
        protected UserId() { }

        public UserId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("invalid guid value", nameof(value));

            Value = value;
        }

        public Guid Value { get; init; }

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(UserId self) => self.Value;
    }
}
