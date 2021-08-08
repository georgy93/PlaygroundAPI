namespace Playground.Messaging.RabbitMq
{
    internal class RabbitMqSettings
    {
        public string ClientProvidedConnectionName { get; set; }

        public int CreateConnectionRetryCount { get; set; }

        public string HostName { get; set; }

        public string Password { get; set; }

        public int PublishRetryCount { get; set; }

        public string UserName { get; set; }
    }
}