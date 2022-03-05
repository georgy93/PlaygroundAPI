namespace Playground.Application.IntegrationEvents.Events;

public record OrderStatusChangedBaseIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; init; }

    public string OrderStatus { get; init; }

    public string BuyerName { get; init; }
}