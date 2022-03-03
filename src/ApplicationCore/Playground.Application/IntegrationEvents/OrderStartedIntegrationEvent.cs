namespace Playground.Application.IntegrationEvents
{
    using Common.Integration;

    public record OrderStartedIntegrationEvent(string UserId) : IntegrationEvent;
}