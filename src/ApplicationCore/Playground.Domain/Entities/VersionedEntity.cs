namespace Playground.Domain.Entities
{
    using Playground.Domain.Entities.Abstract;
    using System.ComponentModel.DataAnnotations;

    public class VersionedEntity : Entity<int>
    {
        //[Timestamp] - Configure with fluent API
        public byte[] RowVersion { get; set; }
    }
}