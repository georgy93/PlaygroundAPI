namespace Playground.Application.IntegrationEvents.Events;

public record OrderStartedIntegrationEvent(long UserId) : IntegrationEvent;