namespace Playground.Messaging.RabbitMq;

internal record RabbitMqSettings
{
    public string ClientProvidedConnectionName { get; init; }

    public int CreateConnectionRetryCount { get; init; }

    public string HostName { get; init; }

    public string Password { get; init; }

    public int PublishRetryCount { get; init; }

    public string UserName { get; init; }
}