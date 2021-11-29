namespace Playground.Persistence.EntityFramework.Repositories
{
    using Abstract;
    using Domain.Entities.Aggregates.BuyerAggregate;
    using Microsoft.EntityFrameworkCore;
    using System.Threading;
    using System.Threading.Tasks;

    internal class BuyerRepository : EFRepository<Buyer>, IBuyerRepository
    {
        public BuyerRepository(AppDbContext appDbContext) : base(appDbContext)
        { }

        public override async Task<Buyer> LoadAsync(int id, CancellationToken cancellationToken) => await DbSet
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}