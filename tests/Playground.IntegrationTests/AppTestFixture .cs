namespace Playground.IntegrationTests
{
    using API;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;

    public class AppTestFixture : WebApplicationFactory<Startup>
    {
        protected override IHostBuilder CreateHostBuilder() => Host
            .CreateDefaultBuilder()
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>().UseTestServer();
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            });
    }
}