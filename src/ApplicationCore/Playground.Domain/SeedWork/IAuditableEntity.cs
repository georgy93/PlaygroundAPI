namespace Playground.Domain.SeedWork;

using Services;

public interface IAuditableEntity
{
    string CreatedBy { get; }

    DateTime Created { get; }

    string LastModifiedBy { get; }

    DateTime LastModified { get; }

    void SetCreationInfo(TimeProvider timeProvider, ICurrentUserService currentUserService);

    void SetUpdationInfo(TimeProvider timeProvider, ICurrentUserService currentUserService);
}