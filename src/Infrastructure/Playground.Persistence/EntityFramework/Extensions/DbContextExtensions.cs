namespace Playground.Persistence.EntityFramework.Extensions
{
    using Domain.Entities.Abstract;
    using MediatR;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class DbContextExtensions
    {
        public static async Task DispatchDomainEventsAsync(this AppDbContext appDbContext, IMediator mediator)
        {
            var domainEntities = appDbContext
                .ChangeTracker
                .Entries<IDomainEntity>()
                .Where(x => x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities
                .ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}