namespace Playground.Domain.SeedWork
{
    using System;

    public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditableEntity
        where TKey : IEquatable<TKey>
    {
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}