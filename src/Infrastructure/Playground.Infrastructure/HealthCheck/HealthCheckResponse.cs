﻿namespace Playground.Infrastructure.HealthCheck
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public record HealthCheckResponse
    {
        public string Status { get; init; }

        public IEnumerable<HealthCheck> Checks { get; init; } = Enumerable.Empty<HealthCheck>();

        public TimeSpan TotalDuration { get; init; }
    }
}