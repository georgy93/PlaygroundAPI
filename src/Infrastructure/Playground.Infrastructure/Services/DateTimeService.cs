namespace Playground.Infrastructure.Services
{
    using Domain.Services;
    using System;

    internal class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.UtcNow;
    }
}