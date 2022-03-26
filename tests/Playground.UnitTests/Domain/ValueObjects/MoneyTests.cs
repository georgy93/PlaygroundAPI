namespace Playground.UnitTests.Domain.ValueObjects;

using Mocks;

public class MoneyTests
{
    private readonly ICurrencyLookupService _currencyLookupService;

    public MoneyTests()
    {
        _currencyLookupService = new MockedCurrencyLookupService();
    }

    [Theory]
    [InlineData(0, "RON")]
    [InlineData(10, "BGN")]
    public void Create_Money_With_Positive_Amount_Should_Succeed(decimal amount, string currency)
    {
        // Arrange
        // Act
        var money = Money.FromAmount(amount, currency, _currencyLookupService);

        // Assert
        Assert.Equal(amount, money.Amount);
        Assert.Equal(currency, money.Currency);
    }

    [Theory]
    [InlineData("ron")]
    [InlineData("bGn")]
    public void Create_Should_Be_Created_With_Upper_Case(string currency)
    {
        // Arrange
        var amount = 10.22m;

        // Act
        var money = Money.FromAmount(amount, currency, _currencyLookupService);

        // Assert
        Assert.Equal(currency.ToUpper(), money.Currency);
    }

    //[Theory]
    //[InlineData(1.23, "RON", "1.23 RON")]
    //[InlineData(14.10, "BGN", "14.10 BGN")]
    //[InlineData(623.33, "USD", "623.33 USD")]
    //public void Money_String_Representation_Should_Be_Correct(decimal amount, string currency, string expectedDisplayValue)
    //{
    //    // Arrange
    //    // Act
    //    var money = Money.FromAmount(amount, currency, _currencyLookupService);

    //    // Assert
    //    Assert.Equal(expectedDisplayValue, money.ToString());
    //}

    [Theory]
    [InlineData(-10.23, "RON")]
    [InlineData(-1.25, "BGN")]
    public void Create_Money_With_Negative_Amount_Should_Throw_Exception(decimal amount, string currency)
    {
        // Arrange
        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => Money.FromAmount(amount, currency, _currencyLookupService));
    }

    [Theory]
    [InlineData(20.00, 11.20)]
    [InlineData(12, 14.50)]
    [InlineData(13.56, 6.44)]
    public void Adding_Money_With_Equal_Currency_Should_Succeed(decimal left, decimal right)
    {
        // Arrange
        const string currency = "BGN";
        var leftMoney = Money.FromAmount(left, currency, _currencyLookupService);
        var rightMoney = Money.FromAmount(right, currency, _currencyLookupService);

        var expectedAmount = left + right;

        // Act
        var newMoney = leftMoney + rightMoney;

        // Assert
        Assert.Equal(expectedAmount, newMoney.Amount);
    }

    [Fact]
    public void Adding_Money_With_Different_Currency_Should_Throw_Exception()
    {
        // Arrange
        var leftMoney = Money.FromAmount(20.00m, "RON", _currencyLookupService);
        var rightMoney = Money.FromAmount(10.00m, "BGN", _currencyLookupService);

        // Act
        // Assert
        Assert.Throws<CurrencyMismatchException>(() => leftMoney + rightMoney);
    }

    [Theory]
    [InlineData(20.00, 11.20)]
    [InlineData(12, 10.50)]
    [InlineData(13.56, 6.44)]
    public void Subtracting_Money_With_Equal_Currency_Should_Succeed(decimal left, decimal right)
    {
        // Arrange
        const string currency = "BGN";
        var leftMoney = Money.FromAmount(left, currency, _currencyLookupService);
        var rightMoney = Money.FromAmount(right, currency, _currencyLookupService);

        var expectedAmount = left - right;

        // Act
        var newMoney = leftMoney - rightMoney;

        // Assert
        Assert.Equal(expectedAmount, newMoney.Amount);
    }

    [Theory]
    [InlineData(10.00, 11.20)]
    [InlineData(12, 14.50)]
    public void Subtracting_Money_When_New_Amount_Becomes_Negative_Should_Throw_Exception(decimal left, decimal right)
    {
        // Arrange
        const string currency = "BGN";
        var leftMoney = Money.FromAmount(left, currency, _currencyLookupService);
        var rightMoney = Money.FromAmount(right, currency, _currencyLookupService);

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => leftMoney - rightMoney);
    }

    [Fact]
    public void Subtracting_Money_With_Different_Currency_Should_Throw_Exception()
    {
        // Arrange
        var leftMoney = Money.FromAmount(20.00m, "RON", _currencyLookupService);
        var rightMoney = Money.FromAmount(10.00m, "BGN", _currencyLookupService);

        // Act
        // Assert
        Assert.Throws<CurrencyMismatchException>(() => leftMoney - rightMoney);
    }
}