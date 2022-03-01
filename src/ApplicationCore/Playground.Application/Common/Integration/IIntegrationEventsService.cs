namespace Playground.Application.Common.Integration
{
    public interface IIntegrationEventsService : IDisposable
    {
        Task PublishThroughEventBusAsync(IntegrationEvent integrationEvent);

        Task SaveEventAndPlaygroundChangesAsync(IntegrationEvent integrationEvent);
    }
}