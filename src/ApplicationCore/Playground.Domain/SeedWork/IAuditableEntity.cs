namespace Playground.Domain.SeedWork
{
    using System;
    using Services;

    public interface IAuditableEntity
    {
        string CreatedBy { get; }

        DateTime Created { get; }

        string LastModifiedBy { get; }

        DateTime LastModified { get; }

        void SetCreationInfo(IDateTimeService dateTimeService, ICurrentUserService currentUserService);

        void SetUpdatationInfo(IDateTimeService dateTimeService, ICurrentUserService currentUserService);
    }
}