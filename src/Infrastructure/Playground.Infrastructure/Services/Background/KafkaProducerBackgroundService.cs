namespace Playground.Infrastructure.Services.Background
{
    using Confluent.Kafka;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class KafkaProducerBackgroundService : BackgroundService
    {
        private readonly Random rnd = new();
        private readonly string _topic;
        private readonly IProducer<Null, int> _producer;
        private readonly ILogger<KafkaProducerBackgroundService> _logger;

        public KafkaProducerBackgroundService(ILogger<KafkaProducerBackgroundService> logger)
        {
            _logger = logger;

            var cfg = new ProducerConfig
            {
                BootstrapServers = "kafka:9092",
                ClientId = "NumbersProducer",
                LingerMs = 0,
                Acks = Acks.None,
                EnableDeliveryReports = false // fire & forget config
            };

            _producer = new ProducerBuilder<Null, int>(cfg)
                .SetErrorHandler((_, error) => Console.WriteLine(error.Reason))
                .Build();

            _topic = "numbers";
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
            var publishIntervalMs = 6000;

            while (!cancellationToken.IsCancellationRequested)
            {
                await _producer.ProduceAsync(_topic, new Message<Null, int> { Value = rnd.Next(0, 101) }, cancellationToken);

                _producer.Poll(TimeSpan.FromSeconds(0));

                await Task.Delay(publishIntervalMs, cancellationToken);
            }
        }
    }
}