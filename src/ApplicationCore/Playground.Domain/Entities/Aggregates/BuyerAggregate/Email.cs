namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using Ardalis.GuardClauses;
    using SeedWork;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Email : ValueObject
    {
        protected Email() { }

        internal Email(string email) => Value = email;

        public string Value { get; private set; }

        public static Email FromString(string email)
        {
            Guard.Against.NullOrWhiteSpace(email, nameof(email), "email is not supplied");

            // TODO: more email validation logic
            if (!new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException($"invalid email value {email} supplied", nameof(email));

            return new(email);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Email self) => self.Value;
    }
}