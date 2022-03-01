namespace Playground.Domain.Entities
{
    public class VersionedEntity : Entity<int>
    {
        //[System.ComponentModel.DataAnnotations.Timestamp] - Configure with fluent API
        public byte[] RowVersion { get; set; }
    }
}