namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;

    public record OrderCancelledDomainEvent(Order Order) : INotification;
}