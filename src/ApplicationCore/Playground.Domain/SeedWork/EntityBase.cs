namespace Playground.Domain.SeedWork
{
    using MediatR;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public abstract class EntityBase : IDomainEntity
    {
        private readonly List<INotification> _domainEvents = new();

        [NotMapped] // TODO: exclude from else where??
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();

        public virtual void ValidateState() { }
    }
}