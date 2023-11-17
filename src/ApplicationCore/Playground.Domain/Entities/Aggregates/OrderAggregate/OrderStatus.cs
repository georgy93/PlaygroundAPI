namespace Playground.Domain.Entities.Aggregates.OrderAggregate;

public class OrderStatus : SmartEnum<OrderStatus>
{
    public static readonly OrderStatus Submitted = new("Submitted", 1);
    public static readonly OrderStatus AwaitingValidation = new("Awaiting Validation", 2);
    public static readonly OrderStatus StockConfirmed = new("Stock Confirmed", 3);
    public static readonly OrderStatus Paid = new("Paid", 4);
    public static readonly OrderStatus Shipped = new("Shipped", 5);
    public static readonly OrderStatus Cancelled = new("Cancelled", 6);

    protected OrderStatus(string name, int value) : base(name, value) { }
}