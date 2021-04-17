namespace Playground.Application
{
    using Common.Services;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            return services.AddSingleton<IDateTimeService, DateTimeService>();
        }
    }
}