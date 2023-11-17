namespace Playground.Domain.Exceptions;

public class CurrencyNotSupportedException(string currencyCode) : BusinessException($"Currency: '{currencyCode}' is not supported!")
{
}