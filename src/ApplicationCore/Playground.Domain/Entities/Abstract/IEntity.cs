namespace Playground.Domain.Entities.Abstract
{
    using System;

    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }

        bool IsTransient();
    }
}