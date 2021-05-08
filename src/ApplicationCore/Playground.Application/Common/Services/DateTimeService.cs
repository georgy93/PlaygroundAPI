namespace Playground.Application.Common.Services
{
    using Interfaces;
    using System;

    public class DateTimeService : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}