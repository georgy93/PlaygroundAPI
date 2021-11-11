namespace Playground.Domain.Entities.Aggregates.BuyerAggregate
{
    using SeedWork;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Email : ValueObject
    {
        protected Email() { }

        internal Email(string email) => Value = email;

        public string Value { get; init; }

        public static Email FromString(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email is not supplied", nameof(email));

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