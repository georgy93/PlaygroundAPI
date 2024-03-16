namespace Playground.API.Behavior.Middlewares;

using Infrastructure.HealthCheck;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Net.Mime;
using Utils.Extensions;

public static class HealthChecksMiddleware
{
    public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app, PathString healthCheckPath) => app
        .UseHealthChecks(healthCheckPath, new HealthCheckOptions()
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = MediaTypeNames.Application.Json;

                var hcReport = HealthCheckHelper.CreateHealthCheckResponse(report).Beautify();

                await context.Response.WriteAsync(hcReport);
            }
        });
}