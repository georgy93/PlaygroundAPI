namespace Playground.Persistence.EntityFramework.EntitiesConfiguration
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
            builder.OwnsOne(order => order.ShippingAddress, a => a.WithOwner());
            builder.OwnsOne(order => order.BillingAddress, a => a.WithOwner());
        }
    }
}