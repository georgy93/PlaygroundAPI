namespace Playground.Domain.Entities.Aggregates.User
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public record Email
    {
        // constructor for ef
        protected Email() { }

        internal Email(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("email is not supplied", nameof(email));

            // TODO: more email validation logic
            if (!new EmailAddressAttribute().IsValid(email))
                throw new ArgumentException($"invalid email value {email} supplied", nameof(email));

            Value = email;
        }

        public string Value { get; internal set; }

        public static Email FromString(string email) => new(email);

        public static implicit operator string(Email self) => self.Value;
    }
}