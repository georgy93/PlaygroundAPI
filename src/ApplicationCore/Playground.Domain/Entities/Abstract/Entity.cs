namespace Playground.Domain.Entities.Abstract
{
    using System.Collections.Generic;

    public abstract class Entity<TKey> : IEntity<TKey>
        where TKey : IEqualityComparer<TKey>
    {
        private int? _requestedHashCode;

        public virtual TKey Id { get; protected set; }

        /// <summary>
        /// Checks whether the entity was just now created. Returns true if the entity was created now and has not yet received an Id.
        /// </summary>
        /// <returns></returns>
        public bool IsTransient() => Equals(Id, default);

        public override bool Equals(object other)
        {
            if (other is null or not Entity<TKey>)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            var otherEntity = (Entity<TKey>)other;

            return !otherEntity.IsTransient() && !IsTransient() && Equals(otherEntity.Id, Id);
        }

        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

            return _requestedHashCode.Value;
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => Equals(left, null)
            ? Equals(right, null)
            : left.Equals(right);

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right);
    }
}