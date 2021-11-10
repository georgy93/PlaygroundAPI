namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;
    using MediatR;

    public record OrderShippedDomainEvent(Order Order) : INotification;
}