namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using SeedWork;
    using System;
    using System.Collections.Generic;

    public class UserId : ValueObject
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

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator Guid(UserId self) => self.Value;
    }
}
