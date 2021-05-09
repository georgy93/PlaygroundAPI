namespace Playground.Infrastructure.Services.Background
{
    using Microsoft.Extensions.Hosting;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    internal class SomeBgService : BackgroundService
    {
        public SomeBgService()
        {
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
        {
            Console.WriteLine("Background Hello!");
        },
        stoppingToken);
    }
}