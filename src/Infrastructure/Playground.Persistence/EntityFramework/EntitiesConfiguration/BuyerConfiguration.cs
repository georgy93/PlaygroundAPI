namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
{
    using Domain.Entities.Aggregates.Buyer;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class BuyerConfiguration : IEntityTypeConfiguration<Buyer>
    {
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("Buyers", AppDbContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .UseHiLo("buyerseq", AppDbContext.DEFAULT_SCHEMA);
        }
    }
}