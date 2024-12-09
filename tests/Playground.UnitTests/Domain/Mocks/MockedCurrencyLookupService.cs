namespace Playground.UnitTests.Domain.Mocks;

internal class MockedCurrencyLookupService : ICurrencyLookupService
{
    public bool IsSupported(string currencyCode) => true;
}