namespace Playground.Infrastructure.HealthCheck
{
    public record HealthCheckResponse
    {
        public string Status { get; init; }

        public IEnumerable<HealthCheck> Checks { get; init; } = [];

        public TimeSpan TotalDuration { get; init; }
    }
}