﻿namespace Playground.Domain.Entities.Aggregates.BuyerAggregate;

using GuardClauses;

public class PaymentMethod : Entity<int>
{
    protected PaymentMethod() { }

    public PaymentMethod(CardType cardType, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, TimeProvider timeProvider)
    {
        CardNumber = Guard.Against.NullOrWhiteSpace(cardNumber);
        SecurityNumber = Guard.Against.NullOrWhiteSpace(securityNumber);
        CardHolderName = Guard.Against.NullOrWhiteSpace(cardHolderName);
        Alias = alias;
        Expiration = Guard.Against.ExpiredCard(expiration, timeProvider);
        CardType = Guard.Against.Null(cardType);
    }

    public string Alias { get; private set; }

    public string CardNumber { get; private set; }

    public string SecurityNumber { get; private set; }

    public string CardHolderName { get; private set; }

    public DateTime Expiration { get; private set; }

    public CardType CardType { get; private set; }

    public bool IsEqualTo(CardType cardType, string cardNumber, DateTime expiration)
        => CardType == cardType && CardNumber == cardNumber && Expiration == expiration;
}