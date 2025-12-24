namespace Playground.API.Behavior.Middlewares;

using Infrastructure.HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;

public static class HealthChecksMiddleware
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    extension(IApplicationBuilder app)
    {
        public IApplicationBuilder UseCustomHealthChecks(PathString healthCheckPath) => app
            .UseHealthChecks(healthCheckPath, new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    var hcReport = HealthCheckHelper.CreateHealthCheckResponse(report);

                    await context.Response.WriteAsJsonAsync(hcReport, DefaultJsonSerializerOptions, context.RequestAborted);
                }
            });
    }
}