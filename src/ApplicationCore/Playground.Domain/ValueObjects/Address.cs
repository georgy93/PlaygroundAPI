namespace Playground.Domain.ValueObjects
{
    public record Address
    {
        public string Street { get; init; }

        public string City { get; init; }

        public string State { get; init; }

        public string Country { get; init; }

        public string ZipCode { get; init; }
    }
}