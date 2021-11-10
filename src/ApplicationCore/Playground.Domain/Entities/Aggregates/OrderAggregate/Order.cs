namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using Events;
    using SeedWork;
    using Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Order : Entity<int>, IAggregateRoot
    {
        private readonly List<OrderItem> _orderItems;

        private string _description;
        private bool _isDraft;

        protected Order()
        {
            _orderItems = new();
            _isDraft = false;
        }

        internal Order(Address shippingAddress, Address billingAddress, DateTime creationDate) : this()
        {
            ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
            BillingAddress = billingAddress ?? throw new ArgumentNullException(nameof(billingAddress));
            OrderDate = creationDate;

            //AddOrderStartedDomainEvent();
        }

        public Address ShippingAddress { get; init; }

        public Address BillingAddress { get; init; }

        public DateTime OrderDate { get; init; }

        public OrderStatus OrderStatus { get; private set; }

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

        public static Order NewDraft() => new() { _isDraft = true };

        public static Order New(IDateTimeService dateTimeService, Address shippingAddress, Address billingAddress)
            => new(shippingAddress, billingAddress, dateTimeService.Now);

        public decimal GetTotal() => _orderItems.Sum(orderItem => orderItem.Units * orderItem.UnitPrice);

        public void SetPaidStatus()
        {
            if (OrderStatus == OrderStatus.StockConfirmed)
            {
                AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));

                OrderStatus = OrderStatus.Paid;
                _description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
            }
        }

        public void SetAwaitingValidationStatus()
        {
            if (OrderStatus == OrderStatus.Submitted)
            {
                AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));
                OrderStatus = OrderStatus.AwaitingValidation;
            }
        }

        public void SetStockConfirmedStatus()
        {
            if (OrderStatus == OrderStatus.AwaitingValidation)
            {
                AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(Id));

                OrderStatus = OrderStatus.StockConfirmed;
                _description = "All the items were confirmed with available stock.";
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

        // TODO Use value object for payment
        private void AddOrderStartedDomainEvent(string userId,
            string userName,
            int cardTypeId,
            string cardNumber,
            string cardSecurityNumber,
            string cardHolderName,
            DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
                                                                      cardNumber, cardSecurityNumber,
                                                                      cardHolderName, cardExpiration);

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