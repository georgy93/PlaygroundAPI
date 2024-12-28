﻿namespace Playground.Messaging.RabbitMq.Concrete;

using Abstract;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System.IO;
using System.Net.Sockets;
using System.Threading;

internal class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;
    private readonly int _retryCount;
    private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    private volatile bool _disposed;

    public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 3)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryCount = retryCount;
    }

    public IConnection Connection { get; private set; }

    public bool IsConnected => Connection is { IsOpen: true } && !_disposed;

    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken)
    {
        if (IsConnected)
            return await Connection.CreateChannelAsync(cancellationToken: cancellationToken);

        throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        try
        {
            if (Connection is not null)
                await Connection.DisposeAsync();
        }
        catch (IOException ex)
        {
            _logger.LogCritical(ex, "{Message}", ex.Message);
        }

        _disposed = true;
    }

    public async Task<bool> TryConnectAsync(CancellationToken cancellationToken)
    {
        if (IsConnected)
            return true;

        _logger.LogInformation("RabbitMQ Client is trying to connect");

        await _semaphoreSlim.WaitAsync(cancellationToken);

        if (IsConnected)
            return true;

        try
        {
            await CreateRetryPolicy().ExecuteAsync(async () => Connection = await _connectionFactory.CreateConnectionAsync(cancellationToken));

            if (IsConnected)
            {
                Connection.ConnectionShutdownAsync += OnConnectionShutdownAsync;
                Connection.CallbackExceptionAsync += OnCallbackExceptionAsync;
                Connection.ConnectionBlockedAsync += OnConnectionBlockedAsync;

                _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", Connection.Endpoint.HostName);

                return true;
            }
            else
            {
                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");

                return false;
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    private AsyncRetryPolicy CreateRetryPolicy() => Policy
        .Handle<SocketException>()
        .Or<BrokerUnreachableException>()
        .WaitAndRetryAsync(_retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
        {
            _logger.LogWarning(ex, "RabbitMQ Client could not connect after {TotalSeconds:n1}s ({Message})", time.TotalSeconds, ex.Message);
        });

    private async Task OnConnectionBlockedAsync(object sender, ConnectionBlockedEventArgs e)
    {
        if (_disposed)
            return;

        _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");

        await TryConnectAsync(CancellationToken.None);
    }

    private async Task OnCallbackExceptionAsync(object sender, CallbackExceptionEventArgs e)
    {
        if (_disposed)
            return;

        _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");

        await TryConnectAsync(CancellationToken.None);
    }

    private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs reason)
    {
        if (_disposed)
            return;

        _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");

        await TryConnectAsync(CancellationToken.None);
    }
}