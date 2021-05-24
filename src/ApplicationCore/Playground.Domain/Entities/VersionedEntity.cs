namespace Playground.Domain.Entities
{
    using Playground.Domain.Entities.Abstract;

    public class VersionedEntity : Entity<int>
    {
        //[System.ComponentModel.DataAnnotations.Timestamp] - Configure with fluent API
        public byte[] RowVersion { get; set; }
    }
}