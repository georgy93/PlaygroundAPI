namespace Playground.Domain.Exceptions
{
    public class CurrencyNotSupportedException : BusinessException
    {
        public CurrencyNotSupportedException(string currencyCode) : base($"Currency: '{currencyCode}' is not supported!")
        { }
    }
}