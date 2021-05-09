namespace Playground.Domain.Entities.Abstract
{
    using MediatR;
    using System.Collections.Generic;

    public interface IDomainEntity
    {
        IReadOnlyCollection<INotification> DomainEvents { get; }

        void AddDomainEvent(INotification eventItem);

        void RemoveDomainEvent(INotification eventItem);

        void ClearDomainEvents();
    }
}