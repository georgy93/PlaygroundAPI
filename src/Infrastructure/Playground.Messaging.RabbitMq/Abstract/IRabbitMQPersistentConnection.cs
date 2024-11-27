namespace Playground.Messaging.RabbitMq.Abstract;

public interface IRabbitMQPersistentConnection : IAsyncDisposable
{
    bool IsConnected { get; }

    Task<bool> TryConnectAsync(CancellationToken cancellationToken);

    Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken);
}