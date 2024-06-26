namespace Playground.Domain.Services;   

public interface ICurrencyLookupService
{
    public bool IsSupported(string currencyCode);
}