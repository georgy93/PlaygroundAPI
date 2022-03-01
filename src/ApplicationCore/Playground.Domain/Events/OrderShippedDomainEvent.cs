namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;

    public record OrderShippedDomainEvent(Order Order) : INotification;
}