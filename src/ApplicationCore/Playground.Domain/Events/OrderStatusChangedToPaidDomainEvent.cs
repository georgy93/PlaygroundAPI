namespace Playground.Domain.Events
{
    using Entities.Aggregates.Order;
    using MediatR;
    using System.Collections.Generic;

    public record OrderStatusChangedToPaidDomainEvent(int OrderId, IEnumerable<OrderItem> OrderItems) : INotification;
}