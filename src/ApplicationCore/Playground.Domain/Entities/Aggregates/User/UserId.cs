namespace Playground.Domain.Entities.Aggregates.User
{
    using System;

    public record UserId
    {
        // constructor for ef
        protected UserId() { }

        public UserId(Guid value)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("invalid guid value", nameof(value));

            Value = value;
        }

        public Guid Value { get; internal set; }

        public override string ToString() => Value.ToString();

        public static implicit operator Guid(UserId self) => self.Value;
    }
}
