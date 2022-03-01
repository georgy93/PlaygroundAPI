namespace Playground.Infrastructure.Services
{
    using Domain.Services;

    internal class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}