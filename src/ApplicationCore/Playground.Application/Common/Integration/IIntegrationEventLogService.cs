namespace Playground.Application.Common.Integration;

using Microsoft.EntityFrameworkCore.Storage;

public interface IIntegrationEventLogService
{
    Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId, CancellationToken cancellationToken);

    Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsFailedToPublishPublishAsync(CancellationToken cancellationToken);

    Task SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction);

    Task SaveEventAndPlaygroundChangesAsync(IntegrationEvent integrationEvent);

    Task MarkEventAsPublishedAsync(Guid eventId);

    Task MarkEventAsInProgressAsync(Guid eventId);

    Task MarkEventAsFailedAsync(Guid eventId);
}