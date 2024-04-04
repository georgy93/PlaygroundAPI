namespace Playground.Persistence.EntityFramework.DbContextInterceptors;

using Domain.SeedWork;
using Domain.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;

internal class AuditableEntitiesInterceptor : ISaveChangesInterceptor
{
    private readonly TimeProvider _dateTimeService;
    private readonly ICurrentUserService _currentUserService;

    public AuditableEntitiesInterceptor(TimeProvider timeProvider, ICurrentUserService currentUserService)
    {
        _dateTimeService = timeProvider;
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
                    entry.Entity.SetCreationInfo(_dateTimeService, _currentUserService);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetUpdatationInfo(_dateTimeService, _currentUserService);
                    break;
                default:
                    continue;
            }
        }
    }
}
