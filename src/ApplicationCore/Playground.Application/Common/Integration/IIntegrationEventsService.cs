namespace Playground.Application.Common.Integration
{
    using System;
    using System.Threading.Tasks;

    public interface IIntegrationEventsService : IDisposable
    {
        Task PublishThroughEventBusAsync(IntegrationEvent integrationEvent);

        Task SaveEventAndPlaygroundChangesAsync(IntegrationEvent integrationEvent);
    }
}