namespace Playground.Persistence.EntityFramework.EntitiesConfiguration.OrderAggreggate;

using Domain.Entities.Aggregates.OrderAggregate;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders", AppDbContext.DEFAULT_SCHEMA);

        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id)
            .UseHiLo("orderseq", AppDbContext.DEFAULT_SCHEMA);

        builder.Property<int>("_versionId").HasColumnName("VersionId").IsConcurrencyToken();

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
    extension(EntityTypeBuilder<Order> builder)
    {
        public EntityTypeBuilder<Order> ConfigureShippingAddress() => builder.OwnsOne(order => order.ShippingAddress, a =>
        {
            var ShippingPrefix = "Shipping";

            a.Property(p => p.Country).HasColumnName(ShippingPrefix + nameof(Address.Country)).HasMaxLength(255).IsRequired();
            a.Property(p => p.City).HasColumnName(ShippingPrefix + nameof(Address.City)).HasMaxLength(255).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName(ShippingPrefix + nameof(Address.ZipCode)).HasMaxLength(255).IsRequired();
            a.Property(p => p.State).HasColumnName(ShippingPrefix + nameof(Address.State)).HasMaxLength(255).IsRequired();
            a.Property(p => p.Street).HasColumnName(ShippingPrefix + nameof(Address.Street)).HasMaxLength(255).IsRequired();
        });

        public EntityTypeBuilder<Order> ConfigureBillingAddress() => builder.OwnsOne(order => order.BillingAddress, a =>
        {
            var BillingPrefix = "Billing";

            a.Property(p => p.Country).HasColumnName(BillingPrefix + nameof(Address.Country)).HasMaxLength(255).IsRequired();
            a.Property(p => p.City).HasColumnName(BillingPrefix + nameof(Address.City)).HasMaxLength(255).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName(BillingPrefix + nameof(Address.ZipCode)).HasMaxLength(255).IsRequired();
            a.Property(p => p.State).HasColumnName(BillingPrefix + nameof(Address.State)).HasMaxLength(255).IsRequired();
            a.Property(p => p.Street).HasColumnName(BillingPrefix + nameof(Address.Street)).HasMaxLength(255).IsRequired();
        });

        public EntityTypeBuilder<Order> ConfigureOrderStatus()
        {
            builder.Property(o => o.OrderStatus)
                .HasColumnName("OrderStatusId")
                .HasConversion(os => os.Value, os => OrderStatus.FromValue(os))
                .IsRequired();

            return builder;
        }

        public EntityTypeBuilder<Order> ConfigureOrderItems()
        {
            builder.HasMany(b => b.OrderItems)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            return builder;
        }

        public EntityTypeBuilder<Order> ConfigureOrderDate()
        {
            builder.Property(o => o.OrderDate)
               .HasColumnName(nameof(Order.OrderDate))
               .IsRequired();

            return builder;
        }
    }
}