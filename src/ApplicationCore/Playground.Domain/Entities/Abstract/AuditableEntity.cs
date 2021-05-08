namespace Playground.Domain.Entities.Abstract
{
    using System;
    using System.Collections.Generic;

    public abstract class AuditableEntity<TKey> : Entity<TKey>
         where TKey : IEqualityComparer<TKey>
    {
        public string CreatedBy { get; set; }

        public DateTime Created { get; set; }

        public string LastModifiedBy { get; set; }

        public DateTime? LastModified { get; set; }
    }
}