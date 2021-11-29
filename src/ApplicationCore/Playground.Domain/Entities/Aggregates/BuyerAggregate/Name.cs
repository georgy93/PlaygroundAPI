namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using Ardalis.GuardClauses;
    using SeedWork;
    using System.Collections.Generic;

    public class Name : ValueObject
    {
        protected Name() { }

        internal Name(string name) => Value = name;

        public string Value { get; private set; }

        public static Name FromString(string name)
        {
            Guard.Against.NullOrWhiteSpace(name, nameof(name), "name is not supplied");

            return new(name);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Name self) => self.Value;
    }
}