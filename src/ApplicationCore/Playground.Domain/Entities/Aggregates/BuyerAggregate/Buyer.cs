namespace Playground.Domain.Entities.Aggregates.BuyerAggregate;

using Events;

public class Buyer : AggregateRootBase<long>
{
    private readonly List<PaymentMethod> _paymentMethods = [];

    // ef wants default constructor or one that accpets values for all parameters. The access modifier is not important
    protected Buyer() { }

    //  TODO: are names and email necessary?
    public Buyer(UserId userId, Email email, FullName fullName)
    {
        UserId = Guard.Against.Null(userId);
        Email = Guard.Against.Null(email);
        FullName = Guard.Against.Null(fullName);
    }

    public UserId UserId { get; private set; }

    public FullName FullName { get; private set; }

    public Email Email { get; private set; }

    public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

    public PaymentMethod VerifyOrAddPaymentMethod(CardType cardType,
                                                  string alias,
                                                  string cardNumber,
                                                  string securityNumber,
                                                  string cardHolderName,
                                                  DateTime expiration,
                                                  int orderId,
                                                  TimeProvider timeProvider)
    {
        Guard.Against.Null(cardType);
        Guard.Against.Null(timeProvider);

        var paymentMethod = _paymentMethods.SingleOrDefault(p => p.IsEqualTo(cardType, cardNumber, expiration));

        if (paymentMethod is null)
        {
            paymentMethod = new PaymentMethod(cardType, alias, cardNumber, securityNumber, cardHolderName, expiration, timeProvider);

            _paymentMethods.Add(paymentMethod);
        }

        AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, paymentMethod, orderId));

        return paymentMethod;
    }
}