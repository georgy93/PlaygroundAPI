namespace Playground.Application.Common.Dtos;

public record AddressDto
{
    public string City { get; init; }

    public string Street { get; init; }

    public string State { get; init; }

    public string Country { get; init; }

    public string ZipCode { get; init; }
}