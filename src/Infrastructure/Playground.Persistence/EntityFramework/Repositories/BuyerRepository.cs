namespace Playground.Persistence.EntityFramework.Repositories
{
    using Abstract;
    using Domain.Entities.Aggregates.BuyerAggregate;

    internal class BuyerRepository : EFRepository<Buyer>, IBuyerRepository
    {
        public BuyerRepository(AppDbContext appDbContext) : base(appDbContext)
        { }
    }
}