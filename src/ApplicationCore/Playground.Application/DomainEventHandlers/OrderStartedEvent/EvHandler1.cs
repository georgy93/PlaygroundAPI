namespace Playground.Application.DomainEventHandlers.OrderStartedEvent
{
    using Domain.Events;
    using MediatR;
    using System.Threading;
    using System.Threading.Tasks;

    public class EvHandler1 : INotificationHandler<OrderStartedDomainEvent>
    {
        public Task Handle(OrderStartedDomainEvent notification, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}