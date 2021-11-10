namespace Playground.Persistence.EntityFramework.Repositories
{
    using Abstract;
    using Domain.Entities.Aggregates.OrderAggregate;

    internal class OrderRepository : EFRepository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext appDbContext) : base(appDbContext)
        { }
    }
}