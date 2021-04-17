namespace Playground.Domain.Entities.Abstract
{
    using System.Collections.Generic;

    public interface IEntity<TKey> where TKey : IEqualityComparer<TKey>
    {
        TKey Id { get; }

        bool IsTransient();
    }
}