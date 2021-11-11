namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using SeedWork;
    using System;
    using System.Collections.Generic;

    public class Name : ValueObject
    {
        protected Name() { }

        internal Name(string name) => Value = name;

        public string Value { get; init; }

        public static Name FromString(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("email is not supplied", nameof(name));

            return new(name);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Name self) => self.Value;
    }
}