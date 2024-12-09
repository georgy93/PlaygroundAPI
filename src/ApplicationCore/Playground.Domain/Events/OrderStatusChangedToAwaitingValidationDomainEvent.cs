namespace Playground.Domain.Events;

public record OrderStatusChangedToAwaitingValidationDomainEvent(int OrderId, IEnumerable<OrderItem> OrderItems) : INotification;