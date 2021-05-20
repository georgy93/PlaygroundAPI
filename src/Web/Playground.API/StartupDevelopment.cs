namespace Playground.API
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Diagnostics;

    /// <summary>
    /// This Startup class will be executed in developmend environment settings
    /// </summary>
    public class StartupDevelopment : Startup
    {
        public StartupDevelopment(IConfiguration configuration, IWebHostEnvironment environment)
            : base(configuration, environment)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            Debug.WriteLine($"Hello from {nameof(ConfigureServices)} in {nameof(StartupDevelopment)}");

            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app)
        {
            Debug.WriteLine($"Hello from {nameof(Configure)} in {nameof(StartupDevelopment)}");

            base.Configure(app);
        }
    }
}