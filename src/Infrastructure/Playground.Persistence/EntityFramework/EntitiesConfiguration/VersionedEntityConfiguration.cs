namespace Playground.Persistence.EntityFramework.EntitiesConfiguration;

using Domain.Entities;

internal class VersionedEntityConfiguration : IEntityTypeConfiguration<VersionedEntity>
{
    public void Configure(EntityTypeBuilder<VersionedEntity> builder)
    {
        // Versioned entity https://www.learnentityframeworkcore.com/concurrency
        builder.Property(x => x.RowVersion).IsRowVersion();
    }
}