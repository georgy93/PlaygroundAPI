namespace Playground.Domain.Services;   

public interface ICurrencyLookupService
{
    bool IsSupported(string currencyCode);
}