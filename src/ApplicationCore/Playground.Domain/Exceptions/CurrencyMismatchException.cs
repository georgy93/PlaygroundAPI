namespace Playground.Domain.Exceptions;

public class CurrencyMismatchException(Money left, Money right)
    : BusinessException($"Currency mismatch. Left currency is: '{left.Currency}'. Right currency is: '{right.Currency}'")
{ }