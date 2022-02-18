namespace Playground.Persistence.EntityFramework.EntitiesConfiguration.OrderAggreggate
{
    using Domain.Entities.Aggregates.OrderAggregate;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders", AppDbContext.DEFAULT_SCHEMA);

            builder.HasKey(order => order.Id);
            builder.Property(order => order.Id)
                .UseHiLo("orderseq", AppDbContext.DEFAULT_SCHEMA);

            builder.Property<string>("_versionId").HasColumnName("VersionId").IsConcurrencyToken();

            // Address value object persisted as owned entity type supported since EF Core 2.0
            // Address class properties will be added as columns to the AggregateRootEntity table thanks to the OwnsOne
            builder
                .ConfigureShippingAddress()
                .ConfigureBillingAddress()
                .ConfigureOrderStatus()
                .ConfigureOrderItems()
                .ConfigureOrderDate();
        }
    }

    internal static class BuyerBuilderExtensions
    {
        public static EntityTypeBuilder<Order> ConfigureShippingAddress(this EntityTypeBuilder<Order> builder) => builder.OwnsOne(order => order.ShippingAddress, a =>
        {
            string ShippingPrefix = "Shipping";

            a.Property(p => p.Country).HasColumnName(ShippingPrefix + nameof(Address.Country)).HasMaxLength(255).IsRequired();
            a.Property(p => p.City).HasColumnName(ShippingPrefix + nameof(Address.City)).HasMaxLength(255).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName(ShippingPrefix + nameof(Address.ZipCode)).HasMaxLength(255).IsRequired();
            a.Property(p => p.State).HasColumnName(ShippingPrefix + nameof(Address.State)).HasMaxLength(255).IsRequired();
            a.Property(p => p.Street).HasColumnName(ShippingPrefix + nameof(Address.Street)).HasMaxLength(255).IsRequired();
        });

        public static EntityTypeBuilder<Order> ConfigureBillingAddress(this EntityTypeBuilder<Order> builder) => builder.OwnsOne(order => order.BillingAddress, a =>
        {
            string BillingPrefix = "Billing";

            a.Property(p => p.Country).HasColumnName(BillingPrefix + nameof(Address.Country)).HasMaxLength(255).IsRequired();
            a.Property(p => p.City).HasColumnName(BillingPrefix + nameof(Address.City)).HasMaxLength(255).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName(BillingPrefix + nameof(Address.ZipCode)).HasMaxLength(255).IsRequired();
            a.Property(p => p.State).HasColumnName(BillingPrefix + nameof(Address.State)).HasMaxLength(255).IsRequired();
            a.Property(p => p.Street).HasColumnName(BillingPrefix + nameof(Address.Street)).HasMaxLength(255).IsRequired();
        });

        public static EntityTypeBuilder<Order> ConfigureOrderStatus(this EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.OrderStatus)
                .HasColumnName("OrderStatusId")
                .HasConversion(os => os.Value, os => OrderStatus.FromValue(os))
                .IsRequired();

            return builder;
        }

        public static EntityTypeBuilder<Order> ConfigureOrderItems(this EntityTypeBuilder<Order> builder)
        {
            builder.HasMany(b => b.OrderItems)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            return builder;
        }

        public static EntityTypeBuilder<Order> ConfigureOrderDate(this EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.OrderDate)
               .HasColumnName(nameof(Order.OrderDate))
               .IsRequired();

            return builder;
        }
    }
}