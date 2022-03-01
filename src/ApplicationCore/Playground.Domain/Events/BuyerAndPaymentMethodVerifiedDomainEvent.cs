namespace Playground.Domain.Events
{
    public record BuyerAndPaymentMethodVerifiedDomainEvent(Buyer Buyer, PaymentMethod Payment, int OrderId) : INotification;
}