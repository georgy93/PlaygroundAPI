namespace Playground.Domain.Events
{
    public record OrderShippedDomainEvent(Order Order) : INotification;
}