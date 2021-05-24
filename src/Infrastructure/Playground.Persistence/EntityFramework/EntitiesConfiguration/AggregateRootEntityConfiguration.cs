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
        }
    }
}