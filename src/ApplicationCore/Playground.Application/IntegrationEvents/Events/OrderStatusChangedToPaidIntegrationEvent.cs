namespace Playground.Application.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; init; }

    public string OrderStatus { get; init; }

    public string BuyerName { get; init; }

    public IEnumerable<OrderStockItem> OrderStockItems { get; init; } = Enumerable.Empty<OrderStockItem>();
}