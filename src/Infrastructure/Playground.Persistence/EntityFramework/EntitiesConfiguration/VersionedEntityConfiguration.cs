namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class VersionedEntityConfiguration : IEntityTypeConfiguration<VersionedEntity>
    {
        public void Configure(EntityTypeBuilder<VersionedEntity> builder)
        {
            // Versioned entity https://www.learnentityframeworkcore.com/concurrency
            builder.Property(x => x.RowVersion).IsRowVersion();
        }
    }
}