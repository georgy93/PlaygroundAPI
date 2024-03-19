namespace Playground.Persistence.EntityFramework.DbContextInterceptors;

using Domain.SeedWork;
using Microsoft.EntityFrameworkCore.Diagnostics;

internal class IncreaseAggregatesVersionInterceptor : ISaveChangesInterceptor
{
    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            IncreaseAggregatesVersions(eventData.Context);
        }

        return new ValueTask<InterceptionResult<int>>(result);
    }

    private static void IncreaseAggregatesVersions(DbContext context)
    {
        foreach (var modifiedAggregateRoot in context.ChangeTracker.Entries<IAggregateRoot>())
        {
            modifiedAggregateRoot.Entity.IncreaseVersion();
        }
    }
}