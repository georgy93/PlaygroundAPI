namespace Playground.Application.Common.Integration
{
    using Microsoft.EntityFrameworkCore.Storage;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

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
}