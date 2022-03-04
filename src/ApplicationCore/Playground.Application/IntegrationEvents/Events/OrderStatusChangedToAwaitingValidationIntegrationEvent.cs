namespace Playground.Application.IntegrationEvents.Events;

public record OrderStatusChangedToAwaitingValidationIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; init; }

    public string OrderStatus { get; init; }

    public string BuyerName { get; init; }

    public IEnumerable<OrderStockItem> OrderStockItems { get; init; } = Enumerable.Empty<OrderStockItem>();
}

public record OrderStockItem(int ProductId, int Units);