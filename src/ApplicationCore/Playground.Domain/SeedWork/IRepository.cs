namespace Playground.Domain.SeedWork;

using System.Threading;
using System.Threading.Tasks;

public interface IRepository<TKey, TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }

    Task<TEntity> LoadAsync(TKey id, CancellationToken cancellationToken);

    ValueTask AddAsync(TEntity entity);

    ValueTask<bool> ExistsAsync(TKey id, CancellationToken cancellationToken);
}