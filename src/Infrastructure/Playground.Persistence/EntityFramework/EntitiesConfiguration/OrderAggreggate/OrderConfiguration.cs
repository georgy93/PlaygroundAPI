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

            // Address value object persisted as owned entity type supported since EF Core 2.0
            // Address class properties will be added as columns to the AggregateRootEntity table thanks to the OwnsOne
            builder
                .ConfigureShippingAddress()
                .ConfigureBillingAddress()
                .ConfigureOrderStatus()
                .ConfigureOrderItems();
        }
    }

    internal static class BuyerBuilderExtensions
    {
        public static EntityTypeBuilder<Order> ConfigureShippingAddress(this EntityTypeBuilder<Order> builder) => builder.OwnsOne(order => order.ShippingAddress, a =>
        {
            a.Property(p => p.Country).HasColumnName("Shipping" + nameof(Address.Country)).HasMaxLength(255).IsRequired();
            a.Property(p => p.City).HasColumnName("Shipping" + nameof(Address.City)).HasMaxLength(255).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName("Shipping" + nameof(Address.ZipCode)).HasMaxLength(255).IsRequired();
            a.Property(p => p.State).HasColumnName("Shipping" + nameof(Address.State)).HasMaxLength(255).IsRequired();
            a.Property(p => p.Street).HasColumnName("Shipping" + nameof(Address.Street)).HasMaxLength(255).IsRequired();
        });

        public static EntityTypeBuilder<Order> ConfigureBillingAddress(this EntityTypeBuilder<Order> builder) => builder.OwnsOne(order => order.BillingAddress, a =>
        {
            a.Property(p => p.Country).HasColumnName("Billing" + nameof(Address.Country)).HasMaxLength(255).IsRequired();
            a.Property(p => p.City).HasColumnName("Billing" + nameof(Address.City)).HasMaxLength(255).IsRequired();
            a.Property(p => p.ZipCode).HasColumnName("Billing" + nameof(Address.ZipCode)).HasMaxLength(255).IsRequired();
            a.Property(p => p.State).HasColumnName("Billing" + nameof(Address.State)).HasMaxLength(255).IsRequired();
            a.Property(p => p.Street).HasColumnName("Billing" + nameof(Address.Street)).HasMaxLength(255).IsRequired();
        });


        public static EntityTypeBuilder<Order> ConfigureOrderStatus(this EntityTypeBuilder<Order> builder)
        {
            builder.Property<int>("_orderStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderStatusId")
                .IsRequired();

            builder.HasOne(p => p.OrderStatus)
                .WithMany()
                .HasForeignKey("_orderStatusId")
                .HasConstraintName("fk_Orders_to_OrderStatuses");

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
    }
}