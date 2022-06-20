namespace Playground.Infrastructure.Services.Background
{
    using Application.Common.Integration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    internal class IntegrationEventsPublisherBackgroundService : BackgroundService
    {
        private readonly ILogger<IntegrationEventsPublisherBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly PeriodicTimer _periodicTimer;

        public IntegrationEventsPublisherBackgroundService(ILogger<IntegrationEventsPublisherBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _periodicTimer = new(TimeSpan.FromSeconds(3));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
        {
            while (await _periodicTimer.WaitForNextTickAsync(stoppingToken) && !stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();

                    await ProcessFailedMessagesAsync(scope, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while publishing failed integration events");
                }
            }

        }, stoppingToken);

        private static async Task ProcessFailedMessagesAsync(IServiceScope serviceScope, CancellationToken cancellationToken)
        {
            var integrationEventsPublisher = serviceScope.ServiceProvider.GetService<IIntegrationEventsService>();
            var integrationEventLogService = serviceScope.ServiceProvider.GetService<IIntegrationEventLogService>();

            var failedIntegrationEventLogEntries = await integrationEventLogService.RetrieveEventLogsFailedToPublishPublishAsync(cancellationToken);

            foreach (var integrationEventLogEntry in failedIntegrationEventLogEntries)
            {
                await integrationEventsPublisher.PublishThroughEventBusAsync(integrationEventLogEntry.IntegrationEvent);
            }
        }
    }
}