namespace Playground.Persistence.EntityFramework.Extensions
{
    using Domain.SeedWork;
    using MediatR;

    internal static class DbContextExtensions
    {
        public static async Task DispatchDomainEventsAsync(this AppDbContext appDbContext, IMediator mediator)
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