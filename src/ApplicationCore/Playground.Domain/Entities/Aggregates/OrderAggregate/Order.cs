namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using Ardalis.GuardClauses;
    using Events;
    using Playground.Domain.Entities.Aggregates.BuyerAggregate;
    using SeedWork;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Order : AggregateRootBase<int>
    {
        private readonly List<OrderItem> _orderItems;

        private string _description;
        private bool _isDraft;

        protected Order()
        {
            _orderItems = new();
            _isDraft = false;
        }

        internal Order(IDateTimeService dateTimeService, Address shippingAddress, Address billingAddress, Buyer buyer, PaymentMethod paymentMethod) : this()
        {
            ShippingAddress = Guard.Against.Null(shippingAddress);
            BillingAddress = Guard.Against.Null(billingAddress);
            OrderDate = Guard.Against.Null(dateTimeService).Now;
            OrderStatus = OrderStatus.Submitted;

            //  _buyerId = buyerId;
            //  _paymentMethodId = paymentMethodId;

            AddOrderStartedDomainEvent(buyer, paymentMethod);
        }

        public static Order NewDraft() => new() { _isDraft = true };

        public static Order New(IDateTimeService dateTimeService, Address shippingAddress, Address billingAddress, Buyer buyer, PaymentMethod paymentMethod)
            => new(dateTimeService, shippingAddress, billingAddress, buyer, paymentMethod);

        public Address ShippingAddress { get; private set; }

        public Address BillingAddress { get; private set; }

        public DateTime OrderDate { get; private set; }

        public OrderStatus OrderStatus { get; private set; }

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public decimal GetTotal() => _orderItems.Sum(orderItem => orderItem.GetTotalWithoutDiscount());

        public void SetPaidStatus()
        {
            if (OrderStatus == OrderStatus.StockConfirmed)
            {
                OrderStatus = OrderStatus.Paid;
                _description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";

                AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));
            }
        }

        public void SetAwaitingValidationStatus()
        {
            if (OrderStatus == OrderStatus.Submitted)
            {
                OrderStatus = OrderStatus.AwaitingValidation;

                AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));
            }
        }

        public void SetStockConfirmedStatus()
        {
            if (OrderStatus == OrderStatus.AwaitingValidation)
            {
                OrderStatus = OrderStatus.StockConfirmed;
                _description = "All the items were confirmed with available stock.";

                AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(Id));
            }
        }

        public void SetShippedStatus()
        {
            if (OrderStatus != OrderStatus.Paid)
                StatusChangeException(OrderStatus.Shipped);

            OrderStatus = OrderStatus.Shipped;
            _description = "The order was shipped.";

            AddDomainEvent(new OrderShippedDomainEvent(this));
        }

        public void SetCancelledStatus()
        {
            if (OrderStatus == OrderStatus.Paid || OrderStatus == OrderStatus.Shipped)
                StatusChangeException(OrderStatus.Cancelled);

            OrderStatus = OrderStatus.Cancelled;
            _description = $"The order was cancelled.";

            AddDomainEvent(new OrderCancelledDomainEvent(this));
        }

        private void AddOrderStartedDomainEvent(Buyer buyer, PaymentMethod paymentMethod)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this, buyer.UserId, buyer.FullName, paymentMethod.CardType.Value,
                                                                      paymentMethod.CardNumber, paymentMethod.SecurityNumber,
                                                                      paymentMethod.CardHolderName, paymentMethod.Expiration);

            AddDomainEvent(orderStartedDomainEvent);
        }

        public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
        {
            if (OrderStatus == OrderStatus.AwaitingValidation)
            {
                OrderStatus = OrderStatus.Cancelled;

                var itemsStockRejectedProductNames = OrderItems
                    .Where(orderItem => orderStockRejectedItems.Contains(orderItem.ProductId))
                    .Select(orderItem => orderItem.ProductName);

                var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);

                _description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
            }
        }

        private void StatusChangeException(OrderStatus orderStatusToChange)
            => throw new InvalidOperationException($"Is not possible to change the order status from {OrderStatus} to {orderStatusToChange}.");
    }
}