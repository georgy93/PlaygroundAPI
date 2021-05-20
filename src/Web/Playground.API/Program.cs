namespace Playground.API
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using Persistence.EntityFramework;
    using Persistence.EntityFramework.Extensions;
    using Serilog;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;
    using Utils.Helpers;

    public static class Program
    {
        // Run as HTTPS https://tomssl.com/how-to-run-asp-net-core-3-1-over-https-in-docker-using-linux-containers/
        public static async Task Main(string[] args)
        {
            var configuration = ConfigurationHelper.BuildConfigurationRoot(args);

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
                webBuilder.UseKestrel();
                //webBuilder.UseUrls("http://*:80", "https://*:443");
                webBuilder.UseIISIntegration();
                // for multiple startups
                webBuilder.UseStartup(Assembly.GetEntryAssembly().FullName);
            })
            .Build();

        private static void InitLogger(IConfiguration configuration) => Log.Logger = new LoggerConfiguration()
            .Enrich
            .FromLogContext()            
            .ReadFrom
            .Configuration(configuration)
            .CreateLogger();    
    }
}