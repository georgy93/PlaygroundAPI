namespace Playground.Domain.Entities.Aggregates.OrderAggregate;

public class Address : ValueObject
{
    // constructor for ef
    protected Address() { }

    public Address(string street, string city, string state, string country, string zipCode)
    {
        Street = Guard.Against.NullOrWhiteSpace(street, message: "invalid street");
        City = Guard.Against.NullOrWhiteSpace(city, message: "invalid city");
        State = Guard.Against.NullOrWhiteSpace(state, message: "invalid state");
        Country = Guard.Against.NullOrWhiteSpace(country, message: "invalid country");
        ZipCode = Guard.Against.NullOrWhiteSpace(zipCode, message: "invalid zipCode");
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