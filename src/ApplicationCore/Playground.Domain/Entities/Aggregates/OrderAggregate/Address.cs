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
            Guard.Against.NullOrWhiteSpace(street, nameof(street), "invalid street");
            Guard.Against.NullOrWhiteSpace(city, nameof(city), "invalid city");
            Guard.Against.NullOrWhiteSpace(state, nameof(state), "invalid state");
            Guard.Against.NullOrWhiteSpace(country, nameof(country), "invalid country");
            Guard.Against.NullOrWhiteSpace(zipCode, nameof(zipCode), "invalid zipCode");

            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
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