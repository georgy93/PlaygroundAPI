namespace Playground.Domain.Events
{
    using Entities.Aggregates.OrderAggregate;
    using MediatR;
    using System.Collections.Generic;

    public record OrderStatusChangedToAwaitingValidationDomainEvent(int OrderId, IEnumerable<OrderItem> OrderItems) : INotification;
}