namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using Ardalis.GuardClauses;
    using BuyerAggregate;
    using Events;
    using SeedWork;
    using Services;

    public class Order : AggregateRootBase<int>
    {
        private readonly List<OrderItem> _orderItems = new();

        private string _description;
        private bool _isDraft;

        protected Order()
        {
            _orderItems = new();
            _isDraft = false;
        }

        internal Order(IDateTimeService dateTimeService, Address shippingAddress, Address billingAddress, long buyerId, FullName buyerName, PaymentMethod paymentMethod) : this()
        {
            ShippingAddress = Guard.Against.Null(shippingAddress);
            BillingAddress = Guard.Against.Null(billingAddress);
            OrderDate = Guard.Against.Null(dateTimeService).Now;
            OrderStatus = OrderStatus.Submitted;
            BuyerId = buyerId;
            PaymentMethodId = Guard.Against.Null(paymentMethod).Id;

            AddOrderStartedDomainEvent(buyerId, buyerName, paymentMethod);
        }

        public static Order NewDraft() => new() { _isDraft = true };

        public static Order New(IDateTimeService dateTimeService, Address shippingAddress, Address billingAddress, long buyerId, FullName buyerName, PaymentMethod paymentMethod)
            => new(dateTimeService, shippingAddress, billingAddress, buyerId, buyerName, paymentMethod);

        public long BuyerId { get; private set; }

        public long PaymentMethodId { get; private set; }

        public Address ShippingAddress { get; private set; }

        public Address BillingAddress { get; private set; }

        public DateTime OrderDate { get; private set; }

        public OrderStatus OrderStatus { get; private set; }

        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

        public decimal GetTotal() => _orderItems.Sum(orderItem => orderItem.GetTotalWithoutDiscount());

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

        public void SetPaidStatus()
        {
            if (OrderStatus == OrderStatus.StockConfirmed)
            {
                OrderStatus = OrderStatus.Paid;
                _description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";

                AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));
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

        private void AddOrderStartedDomainEvent(long buyerId, string buyerName, PaymentMethod paymentMethod)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this, buyerId.ToString(), buyerName, paymentMethod.CardType.Value,
                                                                      paymentMethod.CardNumber, paymentMethod.SecurityNumber,
                                                                      paymentMethod.CardHolderName, paymentMethod.Expiration);

            AddDomainEvent(orderStartedDomainEvent);
        }

        private void StatusChangeException(OrderStatus orderStatusToChange)
            => throw new InvalidOperationException($"Is not possible to change the order status from {OrderStatus} to {orderStatusToChange}.");
    }
}