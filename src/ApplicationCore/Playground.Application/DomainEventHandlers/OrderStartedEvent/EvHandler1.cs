namespace Playground.Application.DomainEventHandlers.OrderStartedEvent
{
    using Domain.Events;

    public class EvHandler1 : INotificationHandler<OrderStartedDomainEvent>
    {
        public Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}