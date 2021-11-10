namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
{
    using Domain.Entities.Aggregates.BuyerAggregate;
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

            builder.OwnsOne(order => order.FirstName, a =>
            {
                a.Property(p => p.Value).HasColumnName(nameof(Buyer.FirstName));
            });

            builder.OwnsOne(order => order.LastName, a =>
            {
                a.Property(p => p.Value).HasColumnName(nameof(Buyer.LastName));
            });

            builder.OwnsOne(order => order.Email);

        }
    }
}