namespace Playground.Utils.Extensions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ReplaceWithSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
            => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton));

        public IServiceCollection ReplaceWithScoped<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
            => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped));

        public IServiceCollection ReplaceWithTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
            => services.Replace(new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient));

        public IServiceCollection RemoveFirstImplementationOf<TService>()
        {
            var descriptor = services.FirstOrDefault(d => d.ImplementationType == typeof(TService));
            if (descriptor is not null)
                services.Remove(descriptor);

            return services;
        }
    }
}