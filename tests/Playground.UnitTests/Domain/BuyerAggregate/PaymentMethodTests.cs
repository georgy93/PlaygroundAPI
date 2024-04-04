namespace Playground.UnitTests.Domain.BuyerAggregate;

using Microsoft.Extensions.Time.Testing;

public class PaymentMethodTests
{
    private readonly TimeProvider _timeProvider;

    public PaymentMethodTests()
    {
        _timeProvider = new FakeTimeProvider();
    }

    [Fact]
    public void Create_Payment_Method_Should_Succeed()
    {
        //Arrange    
        var cardType = CardType.Visa;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(1);

        //Act
        var paymentMethod = new PaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration, _timeProvider);

        //Assert
        Assert.NotNull(paymentMethod);
        Assert.Equal(alias, paymentMethod.Alias);
        Assert.Equal(cardNumber, paymentMethod.CardNumber);
        Assert.Equal(securityNumber, paymentMethod.SecurityNumber);
        Assert.Equal(cardHolderName, paymentMethod.CardHolderName);
        Assert.Equal(expiration, paymentMethod.Expiration);
        Assert.Equal(cardType, paymentMethod.CardType);
    }

    [Fact]
    public void Create_Payment_Method_Expiration_Fail()
    {
        //Arrange    
        var cardType = CardType.MasterCard;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(-1);

        //Act - Assert
        Assert.Throws<CardExpiredException>(() => new PaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration, _timeProvider));
    }

    [Fact]
    public void Payment_Method_IsEqualTo()
    {
        //Arrange    
        var cardType = CardType.MasterCard;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(1);
        var paymentMethod = new PaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration, _timeProvider);

        //Act
        var isEqual = paymentMethod.IsEqualTo(cardType, cardNumber, expiration);

        //Assert
        Assert.True(isEqual);
    }
}
