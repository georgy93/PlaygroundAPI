namespace Playground.Domain.Events
{
    using Entities.Aggregates.Order;
    using MediatR;

    public record OrderCancelledDomainEvent(Order Order) : INotification;
}