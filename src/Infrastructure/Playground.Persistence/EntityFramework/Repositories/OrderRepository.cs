namespace Playground.Persistence.EntityFramework.Repositories
{
    using Abstract;
    using Domain.Entities.Aggregates.OrderAggregate;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

    internal class OrderRepository : EFRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext appDbContext) : base(appDbContext)
        { }

        public override async Task<Order> LoadAsync(int id, CancellationToken cancellationToken) => await DbSet
            .Include(o => o.OrderItems)
            .Include(o => o.OrderStatus)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}