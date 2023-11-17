namespace Playground.API;

using System.Diagnostics;

/// <summary>
/// This Startup class will be executed in production environment settings
/// </summary>
public class StartupProduction : Startup
{
    public StartupProduction(IConfiguration configuration, IWebHostEnvironment environment)
        : base(configuration, environment)
    {
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        Debug.WriteLine($"Hello from {nameof(ConfigureServices)} in {nameof(StartupProduction)}");

        base.ConfigureServices(services);
    }

    public override void Configure(IApplicationBuilder app)
    {
        Debug.WriteLine($"Hello from {nameof(Configure)} in {nameof(StartupProduction)}");

        base.Configure(app);
    }
}