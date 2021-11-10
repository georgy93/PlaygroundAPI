﻿namespace Playground.Domain.SeedWork
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRepository<TEntity> where TEntity : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }

        ValueTask<TEntity> LoadAsync(int id, CancellationToken cancellationToken);

        ValueTask AddAsync(TEntity entity);

        Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    }
}