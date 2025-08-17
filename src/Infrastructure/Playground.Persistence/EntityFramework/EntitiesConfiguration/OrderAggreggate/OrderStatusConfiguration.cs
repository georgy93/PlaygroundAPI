namespace Playground.Persistence.EntityFramework.EntitiesConfiguration.OrderAggreggate;

using Domain.Entities.Aggregates.OrderAggregate;    

internal class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("OrderStatuses", AppDbContext.DEFAULT_SCHEMA);

      //  builder.ConfigureEnumeration();
    }
}