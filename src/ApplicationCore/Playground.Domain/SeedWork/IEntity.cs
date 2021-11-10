namespace Playground.Domain.SeedWork
{
    using System;

    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }

        bool IsTransient();
    }
}