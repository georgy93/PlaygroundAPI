namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using Ardalis.GuardClauses;
    using GuardClauses;
    using SeedWork;

    public class Email : ValueObject
    {
        protected Email() { }

        internal Email(string email)
        {
            Value = Guard.Against.InvalidEmail(email, nameof(email));
        }

        public string Value { get; private set; }

        public static Email FromString(string email) => new(email);

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Email self) => self.Value;
    }
}