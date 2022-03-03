namespace Playground.Application.IntegrationEvents
{
    using Common.Integration;

    public record OrderStartedIntegrationEvent(long UserId) : IntegrationEvent;
}