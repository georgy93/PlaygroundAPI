namespace Playground.Domain.SeedWork;

using Services;

public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
    where TKey : IEquatable<TKey>
{
    public string CreatedBy { get; private set; }

    public DateTime Created { get; private set; }

    public string LastModifiedBy { get; private set; }

    public DateTime LastModified { get; private set; }

    public void SetCreationInfo(TimeProvider timeProvider, ICurrentUserService currentUserService)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        CreatedBy = currentUserService.UserId;
        Created = now;
        LastModifiedBy = currentUserService.UserId;
        LastModified = now;
    }

    public void SetUpdationInfo(TimeProvider timeProvider, ICurrentUserService currentUserService)
    {
        LastModifiedBy = currentUserService.UserId;
        LastModified = timeProvider.GetUtcNow().UtcDateTime;
    }
}