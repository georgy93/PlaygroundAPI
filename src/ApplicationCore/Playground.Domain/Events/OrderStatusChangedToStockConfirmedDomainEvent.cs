namespace Playground.Domain.Events
{
    public record OrderStatusChangedToStockConfirmedDomainEvent(int OrderId) : INotification;
}