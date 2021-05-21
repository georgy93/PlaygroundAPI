namespace Playground.Utils.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using System.Linq;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ReplaceWithSingleton<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));

        public static IServiceCollection ReplaceWithScoped<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));

        public static IServiceCollection ReplaceWithTransient<TService, TImplementation>(this IServiceCollection services)
            where TService : class
            where TImplementation : class, TService
            => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));

        public static IServiceCollection RemoveFirstImplementationOf<TService>(this IServiceCollection services)
        {
            var descriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(TService));
            if (descriptor is not null)
                services.Remove(descriptor);

            return services;
        }
    }
}