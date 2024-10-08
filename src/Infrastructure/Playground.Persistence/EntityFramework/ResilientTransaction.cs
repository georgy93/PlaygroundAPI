﻿namespace Playground.Persistence.EntityFramework
{
    public class ResilientTransaction
    {
        private readonly DbContext _context;

        private ResilientTransaction(DbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();

                await action();

                await transaction.CommitAsync();
            });
        }

        public static ResilientTransaction New(DbContext context) => new(context);
    }
}