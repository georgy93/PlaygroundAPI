namespace Playground.Domain.SeedWork
{
    public abstract class AggregateRootBase<TKey> : Entity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        private int _versionId; // used by entity builder

        void IAggregateRoot.IncreaseVersion() => _versionId++;
    }
}