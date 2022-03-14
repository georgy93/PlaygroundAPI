namespace Playground.Infrastructure.Messaging.Integration
{
    using Application.Common.Integration;
    using Microsoft.Extensions.Logging;
    using Polly;

    internal class IntegrationEventsService : IIntegrationEventsService
    {
        private readonly ILogger<IntegrationEventsService> _logger;
        private readonly IIntegrationEventLogService _integrationEventLogService;

        private volatile bool _isDisposed;

        public IntegrationEventsService(ILogger<IntegrationEventsService> logger, IIntegrationEventLogService integrationEventLogService)
        {
            _logger = logger;
            _integrationEventLogService = integrationEventLogService;
        }

        public async Task PublishThroughEventBusAsync(IntegrationEvent integrationEvent)
        {
            const int retryCount = 2;

            try
            {
                var policy = CreateRetryPolicy(retryCount);

                await _integrationEventLogService.MarkEventAsInProgressAsync(integrationEvent.Id);

                policy.Execute(() =>
                {
                    //var properties = channel.CreateBasicProperties();
                    //properties.DeliveryMode = 2; // persistent

                    _logger.LogTrace("Publishing event to RabbitMQ: {Id}", integrationEvent.Id);

                    //channel.BasicPublish(
                    //    exchange: BROKER_NAME,
                    //    routingKey: eventName,
                    //    mandatory: true,
                    //    basicProperties: properties,
                    //    body: body);
                });

                await _integrationEventLogService.MarkEventAsPublishedAsync(integrationEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Publishing integration event: {Id} - ({IntegrationEvent})", integrationEvent.Id, integrationEvent);

                await _integrationEventLogService.MarkEventAsFailedAsync(integrationEvent.Id);
            }
        }

        public Task SaveEventAndPlaygroundChangesAsync(IntegrationEvent integrationEvent)
        {
            return _integrationEventLogService.SaveEventAndPlaygroundChangesAsync(integrationEvent);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
            { }

            _isDisposed = true;
        }

        private static Policy CreateRetryPolicy(int retryCount) => Policy
            .Handle<Exception>()
            .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}