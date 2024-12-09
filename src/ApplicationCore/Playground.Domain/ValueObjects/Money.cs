namespace Playground.Domain.ValueObjects;

using Services;

public class Money : ValueObject
{
    private string _displayValue;

    /// <summary>
    /// Constructor for EF
    /// </summary>
    protected Money()
    { }

    internal Money(decimal amount, string currency)
    {
        Amount = Guard.Against.Negative(amount, message: "Amount cannot be negative!"); ;
        Currency = Guard.Against.NullOrWhiteSpace(currency, message: "Currency not provided!").ToUpper();
    }

    public static Money FromAmount(decimal amount, string currency, ICurrencyLookupService currencyLookupService) => currencyLookupService.IsSupported(currency)
        ? new Money(amount, currency)
        : throw new CurrencyNotSupportedException(currency);

    public decimal Amount { get; private init; }

    public string Currency { get; private init; }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(_displayValue))
        {
            var formattedAmount = Amount.ToString("0.00");

            _displayValue = string.Join(' ', formattedAmount, Currency);
        }

        return _displayValue;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public static Money operator +(Money left, Money right)
    {
        Guard.Against.Null(left);
        Guard.Against.Null(right);

        ValidateEqualCurrency(left, right);

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        Guard.Against.Null(left);
        Guard.Against.Null(right);

        ValidateEqualCurrency(left, right);

        var newAmount = left.Amount - right.Amount;
        if (newAmount < 0)
            throw new InvalidOperationException("Ammount cannot be negative");

        return new Money(newAmount, left.Currency);
    }

    private static void ValidateEqualCurrency(Money left, Money right)
    {
        if (!string.Equals(left.Currency, right.Currency))
            throw new CurrencyMismatchException(left, right);
    }
}
