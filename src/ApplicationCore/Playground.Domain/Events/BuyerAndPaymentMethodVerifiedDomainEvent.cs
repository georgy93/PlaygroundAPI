namespace Playground.Domain.Events
{
    using Entities.Aggregates.BuyerAggregate;
    using MediatR;

    public class BuyerAndPaymentMethodVerifiedDomainEvent : INotification
    {
        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, int orderId)
        {
            Buyer = buyer;
            Payment = payment;
            OrderId = orderId;
        }

        public Buyer Buyer { get; private set; }

        public PaymentMethod Payment { get; private set; }

        public int OrderId { get; private set; }
    }
}