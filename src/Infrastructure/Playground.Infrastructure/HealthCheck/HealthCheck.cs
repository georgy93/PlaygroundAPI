namespace Playground.Infrastructure.HealthCheck
{
    public record HealthCheck
    {
        public string Status { get; init; }

        public string Component { get; init; }

        public string Description { get; init; }
    }
}