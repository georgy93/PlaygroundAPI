namespace Playground.API;

using Persistence.EntityFramework;
using Persistence.EntityFramework.Extensions;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;
using Utils.Helpers;

public static class Program
{
    // Run as HTTPS https://tomssl.com/how-to-run-asp-net-core-3-1-over-https-in-docker-using-linux-containers/
    public static async Task Main(string[] args)
    {
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        var configuration = ConfigurationHelper.BuildConfigurationRoot(args);

        InitLogger(configuration);
        Log.Information("Starting up");

        try
        {
            var host = CreateHost(args);

            await host.MigrateDataBaseAsync<AppDbContext>();
            await host.MigrateDataBaseAsync<IntegrationEventLogDbContext>();

            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
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
            webBuilder.UseStartup((typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
        })
        .Build();

    // https://www.humankode.com/asp-net-core/logging-with-elasticsearch-kibana-asp-net-core-and-docker
    // https://www.youtube.com/watch?v=0acSdHJfk64&ab_channel=NickChapsas
    private static void InitLogger(IConfiguration configuration) => Log.Logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
        .WriteTo.Console()
        //.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticConfiguration:Uri"])) // e.g. "http://localhost:9200"
        //{
        //    AutoRegisterTemplate = true,
        //    IndexFormat = $"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace(".", "-")}-{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower().Replace(".", "-")}-{DateTime.UtcNow:yyyy-MM}",
        //    NumberOfReplicas = 1,
        //    NumberOfShards = 2,
        //})
        .ReadFrom.Configuration(configuration)
        .CreateLogger();

    private static readonly EventHandler<UnobservedTaskExceptionEventArgs> OnUnobservedTaskException = (obj, eventArgs) =>
    {
        Log.Error(eventArgs.Exception, "UnobservedTaskException caught");

        foreach (var innerException in eventArgs.Exception.InnerExceptions)
        {
            Log.Error(innerException, "UnobservedTaskException inner exception");
        }
    };
}