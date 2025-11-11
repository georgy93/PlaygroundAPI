namespace Playground.Messaging.RabbitMq;

internal static class Extensions
{
    extension(RabbitMqSettings rabbitMqSettings)
    {
        public IConnectionFactory ToConnectionFactory() => new ConnectionFactory
        {
            UserName = rabbitMqSettings.UserName,
            Password = rabbitMqSettings.Password,
            HostName = rabbitMqSettings.HostName,
            ClientProvidedName = rabbitMqSettings.ClientProvidedConnectionName
        };
    }
}