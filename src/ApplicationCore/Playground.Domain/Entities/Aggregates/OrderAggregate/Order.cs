namespace Playground.Domain.Entities.Aggregates.OrderAggregate;

using Events;

public class Order : AggregateRootBase<int>
{
    private readonly HashSet<OrderItem> _orderItems;

    private string _description;
    private bool _isDraft;

    protected Order()
    {
        _orderItems = [];
        _isDraft = false;
    }

    internal Order(TimeProvider timeProvider, Address shippingAddress, Address billingAddress, long buyerId, FullName buyerName, string username, PaymentMethod paymentMethod)
        : this()
    {
        ShippingAddress = Guard.Against.Null(shippingAddress);
        BillingAddress = Guard.Against.Null(billingAddress);
        OrderDate = Guard.Against.Null(timeProvider).GetUtcNow().UtcDateTime;
        OrderStatus = OrderStatus.Submitted;
        BuyerId = buyerId;
        PaymentMethodId = Guard.Against.Null(paymentMethod).Id;

        AddOrderStartedDomainEvent(buyerId, buyerName, paymentMethod);
    }

    public static Order NewDraft() => new() { _isDraft = true };

    public static Order New(TimeProvider timeProvider, Address shippingAddress, Address billingAddress, long buyerId, FullName buyerName, string username,
        PaymentMethod paymentMethod)
        => new(timeProvider, shippingAddress, billingAddress, buyerId, buyerName, username, paymentMethod);

    public long BuyerId { get; private set; }

    public long PaymentMethodId { get; private set; }

    public Address ShippingAddress { get; private set; }

    public Address BillingAddress { get; private set; }

    public DateTime OrderDate { get; private set; }

    public OrderStatus OrderStatus { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal GetTotal() => _orderItems.Sum(orderItem => orderItem.GetTotalWithoutDiscount());

    public void SetBuyerAndPaymentMethod(long buyerId, long paymentMethodId)
    {
        PaymentMethodId = paymentMethodId;
        BuyerId = buyerId;
    }

    public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, Uri pictureUrl, int units = 1)
    {
        var orderItem = _orderItems.SingleOrDefault(o => o.ProductId == productId);
        if (orderItem is null)
        {
            orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            _orderItems.Add(orderItem);

            return;
        }

        if (discount > orderItem.Discount)
            orderItem.SetNewDiscount(discount);

        orderItem.AddUnits(units);
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