namespace Playground.Persistence.EntityFramework
{
    using Application.Interfaces;
    using Domain.Entities;
    using Domain.Entities.Abstract;
    using Extensions;
    using MediatR;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System;
    using System.Data;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Add-Migration Initial
    /// Update-Database
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public const string DEFAULT_SCHEMA = "playground";

        private readonly IMediator _mediator;
        private readonly IDateTimeService _dateTimeService;
        private readonly ICurrentUserService _currentUserService;

        private IDbContextTransaction _currentTransaction;

        protected AppDbContext() { }

        public AppDbContext(DbContextOptions options) : base(options) { }

        public AppDbContext(DbContextOptions options, IMediator mediator, IDateTimeService dateTimeService, ICurrentUserService currentUserService)
            : base(options)
        {
            _mediator = mediator;
            _dateTimeService = dateTimeService;
            _currentUserService = currentUserService;
        }

        public DbSet<RefreshToken> RefreshTokens { get; init; }

        public DbSet<VersionedEntity> VersionedEntities { get; init; }

        public bool HasActiveTransaction => _currentTransaction is not null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.
            await this.DispatchDomainEventsAsync(_mediator);

            AuditEntities();

            // After executing this line all the changes (from the Command Handler and Domain Event Handlers)
            // performed through the DbContext will be committed
            var result = await SaveChangesAsync(cancellationToken);

            return result > 0;
        }

        // Call this only when we are saving outbox messages or persisting operations that have not produced domain events
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Concurrency:
            // https://www.learnentityframeworkcore.com/concurrency
            // https://stackoverflow.com/questions/33928136/mongodb-optimistic-concurrency-control-with-net
            // https://docs.microsoft.com/en-us/ef/core/saving/concurrency
            // https://dotnetcoretutorials.com/2020/07/17/rowversion-vs-concurrencytoken-in-entityframework-efcore/
            // https://github.com/mikeckennedy/optimistic_concurrency_mongodb_dotnet/blob/master/src/MongoDB.Kennedy/ConcurrentDataContext.cs

            AuditEntities();

            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (!HasActiveTransaction)
                return null;

            _currentTransaction = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync();
                transaction.Commit();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                DisposeTransaction();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        private void AuditEntities()
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.Created = _dateTimeService.Now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModified = _dateTimeService.Now;
                        break;
                    default:
                        continue;
                }
            }
        }

        private void DisposeTransaction()
        {
            if (HasActiveTransaction)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}