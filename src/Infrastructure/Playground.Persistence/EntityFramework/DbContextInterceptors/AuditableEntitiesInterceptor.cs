namespace Playground.Persistence.EntityFramework.DbContextInterceptors;

using Domain.SeedWork;
using Domain.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;

internal class AuditableEntitiesInterceptor : ISaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntitiesInterceptor(TimeProvider timeProvider, ICurrentUserService currentUserService)
    {
        _timeProvider = timeProvider;
        _currentUserService = currentUserService;
    }

    public ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntities(eventData.Context);
        }

        return new ValueTask<InterceptionResult<int>>(result);
    }

    private void UpdateAuditableEntities(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreationInfo(_timeProvider, _currentUserService);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetUpdationInfo(_timeProvider, _currentUserService);
                    break;
                default:
                    continue;
            }
        }
    }
}
