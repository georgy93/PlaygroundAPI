namespace Playground.Domain.SeedWork
{
    public interface IEntity<TKey> where TKey : IEquatable<TKey>
    {
        TKey Id { get; }

        bool IsTransient();
    }
}