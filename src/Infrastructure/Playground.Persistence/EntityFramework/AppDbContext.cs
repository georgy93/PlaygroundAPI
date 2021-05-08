namespace Playground.Persistence.EntityFramework
{
    using Application.Interfaces;
    using Extensions;
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using Playground.Domain.Entities.Abstract;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    public class AppDbContext : DbContext
    {
        private readonly IMediator _mediator;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserService _currentUserService;

        public AppDbContext(DbContextOptions options) : base(options) { }

        public AppDbContext(DbContextOptions options, IMediator mediator, IDateTimeService dateTimeService, ICurrentUserService currentUserService) : base(options)
        {
            _mediator = mediator;
            _dateTimeService = dateTimeService;
            _currentUserService = currentUserService;
        }

        protected AppDbContext() { }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            //{
            //    switch (entry.State)
            //    {
            //        case EntityState.Added:
            //            entry.Entity.CreatedBy = _currentUserService.UserId;
            //            entry.Entity.Created = _dateTimeProvider.Now;
            //            break;
            //        case EntityState.Modified:
            //            entry.Entity.LastModifiedBy = _currentUserService.UserId;
            //            entry.Entity.LastModified = _dateTimeProvider.Now;
            //            break;
            //    }
            //}
            // Dispatch Domain Events collection. 
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
            await this.DispatchDomainEventsAsync(_mediator);

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
            // performed through the DbContext will be committed
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }    
}