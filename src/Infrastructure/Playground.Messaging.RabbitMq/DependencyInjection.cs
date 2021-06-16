namespace Playground.Messaging.RabbitMq
{
    using Microsoft.Extensions.DependencyInjection;
    using RabbitMQ.Client;

    public static class DependencyInjection
    {
        public static IServiceCollection AddRabbitMqMessaging(this IServiceCollection services)
        {
            //var factory = new ConnectionFactory
            //{
            //    Uri = new System.Uri("amqp://user:pass@hostName:port/vhost")
            //};
            //IConnection conn = factory.CreateConnection();

            var hcConnectionFactory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest",
                HostName = "rabbitmq",
                ClientProvidedName = "RabbitMqHealthCheck"
            };

            services
                .AddHealthChecks()
                .AddRabbitMQ(sp => hcConnectionFactory);

            return services;
        }
    }
}