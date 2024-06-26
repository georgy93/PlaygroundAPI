﻿namespace Playground.UnitTests.Domain.BuyerAggregate;

using Microsoft.Extensions.Time.Testing;

public class BuyerTests
{
    private static readonly Email DefaultEmail = Email.FromString("ivan.petrov@abv.bg");
    private static readonly UserId DefaultUserId = new(Guid.NewGuid());
    private static readonly FullName DefaultFullName = new("Ivan", "Petrov", "Stoyanov");

    private readonly TimeProvider _timeProvider;

    public BuyerTests()
    {
        _timeProvider = new FakeTimeProvider();
    }

    [Fact]
    public void Create_Buyer_Should_Succeed()
    {
        //Arrange    
        //Act 
        var buyer = new Buyer(DefaultUserId, DefaultEmail, DefaultFullName);

        //Assert
        Assert.NotNull(buyer);
        Assert.Equal(DefaultUserId, buyer.UserId);
        Assert.Equal(DefaultEmail, buyer.Email);
        Assert.Equal(DefaultFullName, buyer.FullName);
    }

    [Fact]
    public void Add_New_PaymentMethod_Should_Succeed_And_Raise_New_Domain_Event()
    {
        //Arrange    
        var alias = "fakeAlias";
        var orderId = 1;
        var cardType = CardType.MasterCard;
        var cardNumber = "12";
        var cardSecurityNumber = "123";
        var cardHolderName = "FakeName";
        var cardExpiration = DateTime.Now.AddYears(1);
        var buyer = new Buyer(DefaultUserId, DefaultEmail, DefaultFullName);

        //Act 
        var result = buyer.VerifyOrAddPaymentMethod(cardType, alias, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration, orderId, _timeProvider);

        //Assert
        Assert.NotNull(result);
        Assert.Single(buyer.DomainEvents);
        Assert.Contains(buyer.DomainEvents, x => x.GetType() == typeof(BuyerAndPaymentMethodVerifiedDomainEvent));
    }

    [Fact]
    public void Add_Payment_When_Payment_Method_Already_Exsists_Should_Return_The_Existing_Payment_Method()
    {
        //Arrange    
        var cardType = CardType.Amex;
        var alias = "fakeAlias";
        var cardNumber = "124";
        var securityNumber = "1234";
        var cardHolderName = "FakeHolderNAme";
        var expiration = DateTime.Now.AddYears(1);
        var orderId = 1;
        var buyer = new Buyer(DefaultUserId, DefaultEmail, DefaultFullName);

        //Act
        var result1 = buyer.VerifyOrAddPaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration, orderId, _timeProvider);
        var result2 = buyer.VerifyOrAddPaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration, orderId, _timeProvider);

        //Assert
        Assert.Equal(result1, result2);
        Assert.Single(buyer.PaymentMethods);
    }
}