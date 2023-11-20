namespace Playground.Domain.SeedWork;

public abstract class Entity<TKey> : EntityBase, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
    private int? _requestedHashCode;

    public virtual TKey Id { get; protected set; }

    /// <summary>
    /// Checks whether the entity was just now created. Returns true if the entity was created now and has not yet received an Id.
    /// </summary>
    /// <returns></returns>
    public bool IsTransient() => Equals(Id, default(TKey));

    public override int GetHashCode()
    {
        // TODO: unchecked????
        if (IsTransient())
            return base.GetHashCode();

        if (!_requestedHashCode.HasValue)
            _requestedHashCode = Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

        return _requestedHashCode.Value;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Entity<TKey>)
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var other = (Entity<TKey>)obj;

        return !other.IsTransient() && !IsTransient() && Equals(other.Id, Id);
    }

    public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => Equals(left, null)
        ? Equals(right, null)
        : left.Equals(right);

    public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right); // use !(left == right) instead of (left != right) because of the == operator !!!
}