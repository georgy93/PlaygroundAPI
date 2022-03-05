namespace Playground.Persistence.EntityFramework.Repositories
{
    using Abstract;
    using Domain.Entities.Aggregates.BuyerAggregate;

    internal class BuyerRepository : EFRepository<long, Buyer>, IBuyerRepository
    {
        public BuyerRepository(AppDbContext appDbContext) : base(appDbContext)
        { }

        public override async Task<Buyer> LoadAsync(long id, CancellationToken cancellationToken) => await DbSet
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}