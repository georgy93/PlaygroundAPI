namespace Playground.Domain.Events
{
    public record OrderStatusChangedToPaidDomainEvent(int OrderId, IEnumerable<OrderItem> OrderItems) : INotification;
}