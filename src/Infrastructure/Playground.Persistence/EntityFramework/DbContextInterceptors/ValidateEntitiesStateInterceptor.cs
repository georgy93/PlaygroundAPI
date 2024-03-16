namespace Playground.Persistence.EntityFramework.DbContextInterceptors;

using Domain.SeedWork;
using Microsoft.EntityFrameworkCore.Diagnostics;

internal class ValidateEntitiesStateInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ValidateEntitiesState(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ValidateEntitiesState(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<EntityBase>())
        {
            entry.Entity.ValidateState();
        }
    }
}