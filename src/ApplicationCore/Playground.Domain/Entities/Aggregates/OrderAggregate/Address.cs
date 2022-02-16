namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using Ardalis.GuardClauses;
    using SeedWork;
    using System.Collections.Generic;

    public class Address : ValueObject
    {
        // constructor for ef
        protected Address() { }

        public Address(string street, string city, string state, string country, string zipCode)
        {
            Street = Guard.Against.NullOrWhiteSpace(street, nameof(street), "invalid street");
            City = Guard.Against.NullOrWhiteSpace(city, nameof(city), "invalid city");
            State = Guard.Against.NullOrWhiteSpace(state, nameof(state), "invalid state");
            Country = Guard.Against.NullOrWhiteSpace(country, nameof(country), "invalid country");
            ZipCode = Guard.Against.NullOrWhiteSpace(zipCode, nameof(zipCode), "invalid zipCode");
        }

        public string Street { get; private set; }

        public string City { get; private set; }

        public string State { get; private set; }

        public string Country { get; private set; }

        public string ZipCode { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Country;
            yield return City;
            yield return ZipCode;
            yield return State;
            yield return Street;
        }
    }
}