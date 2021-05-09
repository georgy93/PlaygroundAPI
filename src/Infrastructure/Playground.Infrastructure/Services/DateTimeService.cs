namespace Playground.Infrastructure.Services
{
    using Application.Interfaces;
    using System;

    internal class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}