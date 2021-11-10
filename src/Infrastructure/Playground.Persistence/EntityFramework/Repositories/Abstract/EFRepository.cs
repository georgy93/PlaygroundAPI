namespace Playground.Persistence.EntityFramework.Repositories.Abstract
{
    using Domain.SeedWork;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Use Repository only for State changing operations. For reading data follow CQRS
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    internal abstract class EFRepository<TEntity> : IRepository<TEntity>
        where TEntity : class, IAggregateRoot
    {
        protected EFRepository(AppDbContext appDbContext)
        {
            UnitOfWork = appDbContext;
            DbSet = appDbContext.Set<TEntity>();
        }

        public IUnitOfWork UnitOfWork { get; }

        protected DbSet<TEntity> DbSet { get; }

        public async ValueTask AddAsync(TEntity entity) => await DbSet.AddAsync(entity);

        public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken) => await DbSet.FindAsync(new object[] { id }, cancellationToken) != null;
        //.AsQueryable()
        //.AnyAsync(order => order.Id == orderId, cancellationToken);

        public async ValueTask<TEntity> LoadAsync(int id, CancellationToken cancellationToken) => await DbSet.FindAsync(new object[] { id }, cancellationToken);
    }
}