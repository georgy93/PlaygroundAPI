namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using System;

    public record Name
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

        public static implicit operator string(Name self) => self.Value;
    }
}