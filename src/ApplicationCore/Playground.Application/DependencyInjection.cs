﻿namespace Playground.Application;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services
            .AddMediatR(typeof(DependencyInjection));
    }
}