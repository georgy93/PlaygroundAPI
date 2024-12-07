namespace Playground.Messaging.Kafka;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddKafkaMessaging(this IServiceCollection services)
    {
        var healthCheckProducerConfiguration = new ProducerConfig
        {
            BootstrapServers = "kafka:9092",
            MessageSendMaxRetries = 0,
            MessageTimeoutMs = 1500,
            RequestTimeoutMs = 1500,
            SocketTimeoutMs = 1500,
        };

        services
            .AddHealthChecks()
            .AddKafka(healthCheckProducerConfiguration, timeout: TimeSpan.FromSeconds(3));

        return services;
    }
}