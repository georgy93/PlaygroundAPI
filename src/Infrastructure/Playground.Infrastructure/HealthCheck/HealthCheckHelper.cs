﻿namespace Playground.Infrastructure.HealthCheck
{
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using System.Linq;

    public static class HealthCheckHelper
    {
        public static HealthCheckResponse CreateHealthCheckResponse(HealthReport report) => new()
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new HealthCheck()
            {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description
            }),
            Duration = report.TotalDuration
        };
    }
}