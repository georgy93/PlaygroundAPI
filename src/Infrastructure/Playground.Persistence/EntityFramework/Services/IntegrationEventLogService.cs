namespace Playground.Persistence.EntityFramework.Services
{
    using Application.Common.Integration;
    using EntityFramework;
    using System.Linq.Expressions;

    internal class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly IntegrationEventLogDbContext _integrationEventLogContext;
        private readonly AppDbContext _appDbContext;

        private volatile bool _isDisposed;

        public IntegrationEventLogService(IntegrationEventLogDbContext integrationEventLogDbContext, AppDbContext appDbContext)
        {
            _integrationEventLogContext = integrationEventLogDbContext ?? throw new ArgumentNullException(nameof(integrationEventLogDbContext));
            _appDbContext = appDbContext;
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId, CancellationToken cancellationToken)
        {
            var tid = transactionId.ToString();

            return await RetrieveEventLogsBasedOnFilterAsync(e => e.TransactionId == tid && e.PublishState == EventState.NotPublished, cancellationToken);
        }

        public Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsFailedToPublishPublishAsync(CancellationToken cancellationToken)
        {
            return RetrieveEventLogsBasedOnFilterAsync(e => e.PublishState == EventState.PublishedFailed, cancellationToken);
        }

        public Task SaveEventAsync(IntegrationEvent integrationEvent, IDbContextTransaction transaction)
        {
            if (transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new IntegrationEventLogEntry(integrationEvent, transaction.TransactionId);

            _integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _integrationEventLogContext.IntegrationEventEntries.Add(eventLogEntry);

            return _integrationEventLogContext.SaveChangesAsync();
        }

        public async Task SaveEventAndPlaygroundChangesAsync(IntegrationEvent integrationEvent)
        {
            // Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            // See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
            await ResilientTransaction.New(_appDbContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                await _appDbContext.SaveChangesAsync(CancellationToken.None);
                await SaveEventAsync(integrationEvent, _appDbContext.Database.CurrentTransaction);
            });
        }

        public Task MarkEventAsPublishedAsync(Guid eventId) => UpdateEventStatusAsync(eventId, EventState.Published);

        public Task MarkEventAsInProgressAsync(Guid eventId) => UpdateEventStatusAsync(eventId, EventState.InProgress);

        public Task MarkEventAsFailedAsync(Guid eventId) => UpdateEventStatusAsync(eventId, EventState.PublishedFailed);

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
                _integrationEventLogContext?.Dispose();

            _isDisposed = true;
        }

        private async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsBasedOnFilterAsync(Expression<Func<IntegrationEventLogEntry, bool>> filter, CancellationToken cancellationToken)
        {
            var result = await _integrationEventLogContext.IntegrationEventEntries
               .Where(filter)
               .ToListAsync(cancellationToken);

            return result
                .OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent());
        }

        private async Task UpdateEventStatusAsync(Guid eventId, EventState status)
        {
            var eventLogEntry = await _integrationEventLogContext.IntegrationEventEntries.SingleAsync(ie => ie.EventId == eventId);

            eventLogEntry.ChangePublishState(status);

            if (status == EventState.InProgress)
                eventLogEntry.IncreaseTimesSent();

            _integrationEventLogContext.IntegrationEventEntries.Update(eventLogEntry);

            await _integrationEventLogContext.SaveChangesAsync();
        }
    }
}