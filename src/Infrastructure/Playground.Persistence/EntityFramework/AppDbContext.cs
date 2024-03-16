namespace Playground.Persistence.EntityFramework
{
    using Application.Common;
    using Ardalis.GuardClauses;
    using Domain.Entities;
    using Domain.Entities.Aggregates.OrderAggregate;
    using Domain.SeedWork;
    using Extensions;
    using MediatR;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using System.Data;
    using System.Reflection;

    /// <summary>
    /// Add-Migration Initial
    /// Update-Database
    /// https://docs.microsoft.com/en-us/ef/core/modeling/constructors readonly props
    /// </summary>
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IUnitOfWork
    {
        public const string DEFAULT_SCHEMA = "playground";

        private readonly IMediator _mediator;

        private IDbContextTransaction _currentTransaction;

        protected AppDbContext() { }

        public AppDbContext(DbContextOptions options) : base(options) { }

        public AppDbContext(DbContextOptions options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<RefreshToken> RefreshTokens { get; init; }

        public DbSet<Order> Orders { get; init; }

        public DbSet<VersionedEntity> VersionedEntities { get; init; }

        public bool HasActiveTransaction => _currentTransaction is not null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken)
        {
            // Dispatch Domain Events collection.
            // Choices:
            // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including
            // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
            // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions.
            // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers.
            await this.DispatchDomainEventsAsync(_mediator);

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

            IncreaseModifiedAggregatesVersion();

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
            Guard.Against.Null(transaction);

            if (transaction != _currentTransaction)
                throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

            try
            {
                await SaveChangesAsync(CancellationToken.None);
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }

        private void IncreaseModifiedAggregatesVersion()
        {
            var modifiedAggregateRoots = ChangeTracker.Entries<IAggregateRoot>().Where(entry => entry.Entity.DomainEvents.Any());

            foreach (var modifiedAggregateRoot in modifiedAggregateRoots)
                modifiedAggregateRoot.Entity.IncreaseVersion();
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