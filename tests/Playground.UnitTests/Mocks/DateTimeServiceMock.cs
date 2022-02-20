namespace Playground.UnitTests.Mocks
{
    using System;
    using Playground.Domain.Services;

    internal class DateTimeServiceMock : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}