namespace Playground.Messaging.Kafka
{
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddKafkaMessaging(this IServiceCollection services)
        {
            //services
            //    .AddHealthChecks()
            //    .AddKafka();

            return services;
        }
    }
}