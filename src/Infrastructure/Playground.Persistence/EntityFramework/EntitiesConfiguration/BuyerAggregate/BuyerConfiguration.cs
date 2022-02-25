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

            builder.Property<int>("_versionId").HasColumnName("VersionId").IsConcurrencyToken();

            builder
                .ConfigureFullName()
                .ConfigureEmail();
        }
    }

    internal static class BuyerBuilderExtensions
    {
        public static EntityTypeBuilder<Buyer> ConfigureFullName(this EntityTypeBuilder<Buyer> builder) => builder.OwnsOne(buyer => buyer.FullName, a =>
        {
            a.Property(fn => fn.FirstName).HasColumnName(nameof(Buyer.FullName.FirstName)).HasMaxLength(255).IsRequired();
            a.Property(fn => fn.Surname).HasColumnName(nameof(Buyer.FullName.Surname)).HasMaxLength(255).IsRequired();
            a.Property(fn => fn.LastName).HasColumnName(nameof(Buyer.FullName.LastName)).HasMaxLength(255).IsRequired();
        });

        public static EntityTypeBuilder<Buyer> ConfigureEmail(this EntityTypeBuilder<Buyer> builder) => builder.OwnsOne(buyer => buyer.Email, a =>
        {
            a.Property(p => p.Value)
                .HasColumnName(nameof(Buyer.Email))
                .HasMaxLength(255)
                .IsRequired();

            a.HasIndex(e => e.Value)
                .HasDatabaseName("unique_buyers_email_idx")
                .IsUnique();
        });
    }
}