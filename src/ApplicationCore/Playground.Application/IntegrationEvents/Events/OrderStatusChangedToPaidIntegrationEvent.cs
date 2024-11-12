namespace Playground.Application.IntegrationEvents.Events;

public record OrderStatusChangedToPaidIntegrationEvent : OrderStatusChangedBaseIntegrationEvent
{
    public IEnumerable<OrderStockItem> OrderStockItems { get; init; } = [];
}