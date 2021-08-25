namespace Playground.Domain.Events
{
    using MediatR;

    public record OrderStatusChangedToStockConfirmedDomainEvent(int OrderId) : INotification;
}