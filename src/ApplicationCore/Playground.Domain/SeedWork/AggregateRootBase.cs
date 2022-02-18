namespace Playground.Domain.SeedWork
{
    using System;

    public abstract class AggregateRootBase<TKey> : Entity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        private int _versionId; // used by entity builder

        void IAggregateRoot.IncreaseVersion() => _versionId++;
    }
}