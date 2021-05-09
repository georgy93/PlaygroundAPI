namespace Playground.Domain.Entities.Abstract
{
    using System;

    public interface IEntity<TKey> : IEquatable<Entity<TKey>>
    {
        TKey Id { get; }

        bool IsTransient();
    }
}