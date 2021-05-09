namespace Playground.Domain.Entities.Abstract
{
    using MediatR;
    using System.Collections.Generic;

    public abstract class Entity<TKey> : IEntity<TKey>, IDomainEntity
    {
        private readonly List<INotification> _domainEvents = new();

        private int? _requestedHashCode;

        public virtual TKey Id { get; protected set; }

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Checks whether the entity was just now created. Returns true if the entity was created now and has not yet received an Id.
        /// </summary>
        /// <returns></returns>
        public bool IsTransient() => Equals(Id, default);

        public void AddDomainEvent(INotification eventItem) => _domainEvents.Add(eventItem);

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents.Clear();

        public override bool Equals(object other) => Equals(other as Entity<TKey>);

        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }

        public bool Equals(Entity<TKey> other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return !other.IsTransient() && !IsTransient() && Equals(other.Id, Id);
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => Equals(left, null)
            ? Equals(right, null)
            : left.Equals(right);

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right); // use !(left == right) instead of (left != right) because of the == operator !!!
    }
}