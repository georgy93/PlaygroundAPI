namespace Playground.Domain.Entities.Abstract
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        //Task<bool> SaveEntitiesAsync();
        //Task StartAsync();
        //Task CommitAsync();
        //Task RollbackAsync();

        //public async Task<bool> SaveEntitiesAsync()
        //{
        //    try
        //    {
        //        return await SaveChangesAsync() > 0;
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        foreach (var entry in ex.Entries)
        //        {
        //            entry.State = EntityState.Detached;
        //        }
        //        throw;
        //    }
        //}

        //public async Task StartAsync()
        //{
        //    await Database.BeginTransactionAsync();
        //}

        //public async Task CommitAsync()
        //{
        //    Database.CommitTransaction();
        //}

        //public async Task RollbackAsync()
        //{
        //    Database.RollbackTransaction();
        //}
    }
}