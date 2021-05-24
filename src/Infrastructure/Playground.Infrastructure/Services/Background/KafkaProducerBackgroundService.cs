namespace Playground.Infrastructure.Services.Background
{
    using Confluent.Kafka;
    using Domain.ValueObjects;
    using Messaging.Kafka.Serialization.Json;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class KafkaProducerBackgroundService : BackgroundService
    {
        private readonly Random rnd = new();
        private readonly string _topic;
        private readonly IProducer<Null, Ping> _producer;
        private readonly ILogger<KafkaProducerBackgroundService> _logger;

        public KafkaProducerBackgroundService(ILogger<KafkaProducerBackgroundService> logger)
        {
            _logger = logger;

            var cfg = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                ClientId = "PingProducer",
                LingerMs = 0,
                Acks = Acks.None,
                EnableDeliveryReports = false // fire & forget config
            };

            _producer = new ProducerBuilder<Null, Ping>(cfg)
                .SetValueSerializer(new JsonMessageSerializer<Ping>())
                .SetErrorHandler((_, error) => Console.WriteLine(error.Reason))
                .Build();

            _topic = "ping-pong";
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(async () =>
        {
            try
            {
                await StartProducerLoopAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                throw;
            }
            finally
            {
                _producer.Dispose();
            }
        },
        stoppingToken);

        private async Task StartProducerLoopAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var ping = new Ping(rnd.Next(1, 101), DateTime.UtcNow);

                await _producer.ProduceAsync(_topic, new Message<Null, Ping> { Value = ping }, cancellationToken);

                _producer.Poll(TimeSpan.FromSeconds(0));

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
            }
        }
    }
}