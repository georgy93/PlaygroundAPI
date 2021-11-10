namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;
    using MediatR;

    public record OrderCancelledDomainEvent(Order Order) : INotification;
}