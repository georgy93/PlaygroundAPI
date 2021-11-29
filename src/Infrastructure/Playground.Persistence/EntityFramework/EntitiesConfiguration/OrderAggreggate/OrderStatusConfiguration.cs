namespace Playground.Persistence.EntityFramework.EntitiesConfiguration.OrderAggreggate
{
    using Domain.Entities.Aggregates.OrderAggregate;
    using Extensions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    internal class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.ToTable("OrderStatuses", AppDbContext.DEFAULT_SCHEMA);

            builder.ConfigureEnumeration();
        }
    }
}