namespace Playground.Infrastructure.HealthCheck
{
    public record HealthCheckResponse
    {
        public string Status { get; init; }

        public IEnumerable<HealthCheck> Checks { get; init; } = Enumerable.Empty<HealthCheck>();

        public TimeSpan TotalDuration { get; init; }
    }
}