﻿namespace Playground.Infrastructure.Services.Background
{
    using Confluent.Kafka;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class KafkaConsumerBackgroundService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerBackgroundService> _logger;
        private readonly string _topic;
        private readonly IConsumer<Null, int> _consumer;

        public KafkaConsumerBackgroundService(ILogger<KafkaConsumerBackgroundService> logger)
        {
            var consumerConfig = new ConsumerConfig
            {
                EnableAutoCommit = false,
                BootstrapServers = "kafka:9092",
                GroupId = $"numbersConsumer-{Guid.NewGuid()}",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                FetchWaitMaxMs = 1,
                FetchErrorBackoffMs = 1, // retry immediately in case of fetch error
            };

            _consumer = new ConsumerBuilder<Null, int>(consumerConfig).Build();
            _logger = logger;
            _topic = "numbers";
        }

        public int MessagesConsumed { get; private set; }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
        {
            using (_consumer)
            {
                _consumer.Subscribe(_topic);

                try
                {
                    StartConsumerLoop(stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Consumer stopped");
                }
                finally
                {
                    // Ensure the consumer leaves the group cleanly and final offsets are committed
                    _consumer.Close();
                }
            }
        }, stoppingToken);

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    if (consumeResult.IsPartitionEOF)
                        continue;

                    MessagesConsumed++;

                    HandleResult(consumeResult.Message.Value);
                }
                catch (ConsumeException consumeEx)
                {
                    _logger.LogError(consumeEx, $"Consumer for topic '{consumeEx.ConsumerRecord.Topic}'. Error: {consumeEx.Error.Reason}");
                }
            }
        }

        private void HandleResult(int number)
        {
            try
            {
                _logger.LogInformation($"{DateTime.Now}: Received {number}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured");
            }
        }
    }
}