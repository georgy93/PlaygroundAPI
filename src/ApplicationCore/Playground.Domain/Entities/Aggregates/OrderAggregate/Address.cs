namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using SeedWork;
    using System;
    using System.Collections.Generic;

    public class Address : ValueObject
    {
        // constructor for ef
        protected Address() { }

        public Address(string street, string city, string state, string country, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("invalid street", nameof(street));

            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("invalid city", nameof(city));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("invalid state", nameof(state));

            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("invalid country", nameof(country));

            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("invalid zipCode", nameof(zipCode));

            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
        }

        public string Street { get; init; }

        public string City { get; init; }

        public string State { get; init; }

        public string Country { get; init; }

        public string ZipCode { get; init; }

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