namespace Playground.Messaging.RabbitMq
{
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
        {
            //services
            //    .AddHealthChecks()
            //    .AddRabbitMQ();

            return services;
        }
    }
}