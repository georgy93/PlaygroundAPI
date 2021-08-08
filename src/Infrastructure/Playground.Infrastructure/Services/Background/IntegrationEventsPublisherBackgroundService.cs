namespace Playground.Infrastructure.Services.Background
{
    using Application.Common.Integration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class IntegrationEventsPublisherBackgroundService : BackgroundService
    {
        private readonly ILogger<IntegrationEventsPublisherBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IntegrationEventsPublisherBackgroundService(ILogger<IntegrationEventsPublisherBackgroundService> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
        {
            while (!stoppingToken.IsCancellationRequested)
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

                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
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