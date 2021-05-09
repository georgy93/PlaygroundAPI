namespace Playground.API.Behavior.Middlewares
{
    using Infrastructure.HealthCheck;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Http;
    using System.Text.Json;

    public static class HealthChecksMiddleware
    {
        public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app) => app
            .UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var hcReport = JsonSerializer.Serialize(HealthCheckHelper.CreateHealthCheckResponse(report));

                    await context.Response.WriteAsync(hcReport);
                }
            });
    }
}