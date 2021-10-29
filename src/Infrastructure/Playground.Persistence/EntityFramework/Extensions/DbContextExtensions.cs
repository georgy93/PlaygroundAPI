namespace Playground.Persistence.EntityFramework.Extensions
{
    using Domain.Entities.Abstract;
    using MediatR;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class DbContextExtensions
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, AppDbContext appDbContext)
        {
            var domainEntitiesWithPendingEvents = appDbContext
                .ChangeTracker
                .Entries<IDomainEntity>()
                .Where(entry => entry.Entity.DomainEvents.Any());

            var domainEvents = domainEntitiesWithPendingEvents
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            domainEntitiesWithPendingEvents
                .ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (INotification domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
    }
}