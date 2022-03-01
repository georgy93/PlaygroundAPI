namespace Playground.Domain.Events
{
    public record OrderCancelledDomainEvent(Order Order) : INotification;
}