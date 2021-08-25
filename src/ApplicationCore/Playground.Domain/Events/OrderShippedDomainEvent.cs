namespace Playground.Domain.Events
{
    using Entities.Aggregates.Order;
    using MediatR;

    public record OrderShippedDomainEvent(Order Order) : INotification;
}