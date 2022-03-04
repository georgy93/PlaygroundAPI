namespace Playground.Application.IntegrationEvents.Events;

public record OrderStatusChangedToShippedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; init; }

    public string OrderStatus { get; init; }

    public string BuyerName { get; init; }
}