﻿namespace Playground.Domain.SeedWork
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        Task<TEntity> LoadAsync(long id, CancellationToken cancellationToken);

        ValueTask AddAsync(TEntity entity);

        ValueTask<bool> ExistsAsync(long id, CancellationToken cancellationToken);
    }
}