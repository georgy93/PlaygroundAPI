﻿namespace Playground.Persistence.EntityFramework.DbContextInterceptors;

using Domain.SeedWork;
using Microsoft.EntityFrameworkCore.Diagnostics;

internal class ValidateEntitiesStateInterceptor : ISaveChangesInterceptor
{
    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            ValidateEntitiesStates(eventData.Context);
        }

        return new ValueTask<InterceptionResult<int>>(result);
    }

    private static void ValidateEntitiesStates(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<EntityBase>())
        {
            entry.Entity.ValidateState();
        }
    }
}