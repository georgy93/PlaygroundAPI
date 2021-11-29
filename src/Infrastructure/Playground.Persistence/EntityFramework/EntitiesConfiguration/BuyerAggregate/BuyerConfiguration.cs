namespace Playground.Persistence.EntityFramework.EntitiesConfiguration.BuyerAggregate
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

            builder
                .ConfigureFirstName()
                .ConfigureLastName()
                .ConfigureEmail();
        }
    }

    internal static class BuyerBuilderExtensions
    {
        public static EntityTypeBuilder<Buyer> ConfigureFirstName(this EntityTypeBuilder<Buyer> builder) => builder.OwnsOne(buyer => buyer.FirstName, a =>
        {
            a.Property(p => p.Value).HasColumnName(nameof(Buyer.FirstName)).HasMaxLength(255).IsRequired();
        });

        public static EntityTypeBuilder<Buyer> ConfigureLastName(this EntityTypeBuilder<Buyer> builder) => builder.OwnsOne(buyer => buyer.LastName, a =>
        {
            a.Property(p => p.Value).HasColumnName(nameof(Buyer.LastName)).HasMaxLength(255).IsRequired();
        });

        public static EntityTypeBuilder<Buyer> ConfigureEmail(this EntityTypeBuilder<Buyer> builder) => builder.OwnsOne(buyer => buyer.Email, a =>
        {
            a.Property(p => p.Value)
                .HasColumnName(nameof(Buyer.Email))
                .HasMaxLength(255)
                .IsRequired();

            a.HasIndex(e => e.Value)
                .HasDatabaseName("unique_Buyers_Email_idx")
                .IsUnique();
        });
    }
}