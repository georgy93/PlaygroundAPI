namespace Playground.UnitTests.Mocks
{
    internal class DateTimeServiceMock : IDateTimeService
    {
        public DateTime Now => DateTime.Now;
    }
}