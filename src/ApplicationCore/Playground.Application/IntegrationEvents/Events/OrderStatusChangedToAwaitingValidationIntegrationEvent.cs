namespace Playground.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent : OrderStatusChangedBaseIntegrationEvent
{
    public IEnumerable<OrderStockItem> OrderStockItems { get; init; } = Enumerable.Empty<OrderStockItem>();
}

public record OrderStockItem(int ProductId, int Units);