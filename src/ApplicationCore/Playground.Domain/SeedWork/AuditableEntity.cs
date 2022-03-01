namespace Playground.Domain.SeedWork
{
    using Services;

    public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
        where TKey : IEquatable<TKey>
    {
        public string CreatedBy { get; private set; }

        public DateTime Created { get; private set; }

        public string LastModifiedBy { get; private set; }

        public DateTime LastModified { get; private set; }

        public void SetCreationInfo(IDateTimeService dateTimeService, ICurrentUserService currentUserService)
        {
            CreatedBy = currentUserService.UserId;
            Created = dateTimeService.Now;
            LastModifiedBy = currentUserService.UserId;
            LastModified = dateTimeService.Now;
        }

        public void SetUpdatationInfo(IDateTimeService dateTimeService, ICurrentUserService currentUserService)
        {
            LastModifiedBy = currentUserService.UserId;
            LastModified = dateTimeService.Now;
        }
    }
}