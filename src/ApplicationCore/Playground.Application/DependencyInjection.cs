namespace Playground.Application;

using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{

    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            return services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        }
    }
}