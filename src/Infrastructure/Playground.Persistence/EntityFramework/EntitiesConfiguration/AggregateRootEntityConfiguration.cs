namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
{
    using Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class AggregateRootEntityConfiguration : IEntityTypeConfiguration<AggregateRootEntity>
    {
        public void Configure(EntityTypeBuilder<AggregateRootEntity> builder)
        {
            builder.HasKey(x => x.Id);

            // Address value object persisted as owned entity type supported since EF Core 2.0
            // Address class properties will be added as columns to the AggregateRootEntity table thanks to the OwnsOne
            builder.OwnsOne(x => x.Address);
        }
    }
}