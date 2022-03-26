namespace Playground.Domain.Exceptions
{
    public class CurrencyMismatchException : BusinessException
    {
        public CurrencyMismatchException(Money left, Money right) 
            : base($"Currency mismatch. Left currency is: '{left.Currency}'. Right currency is: '{right.Currency}'")
        { }
    }
}