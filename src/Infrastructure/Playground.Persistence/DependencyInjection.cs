namespace Playground.Persistence
{
    using EntityFramework;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config) => services
            .AddEntityFramework(config)
            .AddDataStoresHealthChecks();

        private static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration config)
        {
            // https://github.com/dotnet/aspnetcore/issues/17093
            return services
                .AddDbContext<AppDbContext>(contextBuilder =>
                {
                    contextBuilder.UseSqlServer(
                      connectionString: config.GetConnectionString("DefaultConnection"),
                      sqlServerOptionsAction: sqlServerContextBuilder =>
                      {
                          sqlServerContextBuilder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                          //sqlServerContextBuilder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                      });
                },
                ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
            );
        }

        // https://www.youtube.com/watch?v=bdgtYbGYsK0
        private static IServiceCollection AddDataStoresHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddDbContextCheck<AppDbContext>();

            return services;
        }
    }
}