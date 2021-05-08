namespace Playground.API
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Persistence.EntityFramework;
    using Persistence.EntityFramework.Extensions;
    using Serilog;
    using System;
    using System.IO;
    using System.Threading.Tasks;

    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetConfiguration();

            InitLogger(configuration);
            Log.Information("Starting up");

            try
            {
                var host = CreateHost(args);

                await host.MigrateDataBaseAsync<AppDbContext>();
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHost CreateHost(string[] args) => Host
            .CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .Build();

        private static void InitLogger(IConfiguration configuration) => Log.Logger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()
            .WriteTo
            .Console()
            .ReadFrom
            .Configuration(configuration)
            .CreateLogger();

        // TODO get the better method
        private static IConfiguration GetConfiguration() => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}