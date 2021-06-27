namespace Playground.IntegrationTests
{
    using API;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Hosting;
    using Persistence.EntityFramework;

    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder() => Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll(typeof(AppDbContext));
                    services.AddDbContext<AppDbContext>(opts => opts.UseInMemoryDatabase("IntegrationTestsDb"));
                });
                builder.UseStartup<Startup>().UseTestServer();

            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });
    }
}