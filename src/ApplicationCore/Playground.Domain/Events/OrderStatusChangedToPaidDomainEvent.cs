namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;

    public record OrderStatusChangedToPaidDomainEvent(int OrderId, IEnumerable<OrderItem> OrderItems) : INotification;
}