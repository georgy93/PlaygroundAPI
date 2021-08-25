namespace Playground.Domain.Entities.Aggregates.Buyer
{
    using System;

    public record Name
    {
        protected Name() { }

        internal Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("email is not supplied", nameof(name));

            Value = name;
        }

        public string Value { get; init; }

        public static Email FromString(string name) => new(name);

        public static implicit operator string(Name self) => self.Value;
    }
}