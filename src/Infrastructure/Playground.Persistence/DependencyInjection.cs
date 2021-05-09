namespace Playground.Persistence
{
    using Domain.Entities;
    using EntityFramework;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Mongo;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.Core.Events;
    using System.Diagnostics;

    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config) => services
            .AddEntityFramework(config)
            .AddMongoDatabase(config)
            .AddDataStoresHealthChecks();

        private static IServiceCollection AddEntityFramework(this IServiceCollection services, IConfiguration config)
        {
            // https://github.com/dotnet/aspnetcore/issues/17093
            services
               .AddDbContext<AppDbContext>(
                   contextBuilder =>
                   {
                       contextBuilder.UseSqlServer(
                         connectionString: config.GetConnectionString("DefaultConnection"),
                         sqlServerOptionsAction: sqlServerContextBuilder =>
                         {
                             sqlServerContextBuilder.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                             //sqlServerContextBuilder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         });
                   },
                   contextLifetime: ServiceLifetime.Scoped //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
               )  
               .AddIdentity<ApplicationUser, IdentityRole>()
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<AppDbContext>();

            return services;

            //return services
            //    .AddScoped(typeof(IEfRepository<>), typeof(EfRepository<>))
            //    .AddScoped(typeof(IEfReadOnlyRepository<>), typeof(EfReadOnlyRepository<>));
        }

        // https://www.youtube.com/watch?v=bdgtYbGYsK0
        private static IServiceCollection AddDataStoresHealthChecks(this IServiceCollection services)
        {
            services
                .AddHealthChecks()
                .AddDbContextCheck<AppDbContext>();

            return services;
        }

        private static IServiceCollection AddMongoDatabase(this IServiceCollection services, IConfiguration config)
        {
            return services.AddSingleton(provider =>
            {
                var mongoConfig = provider.GetRequiredService<IOptions<MongoDbSettings>>();
                EntitiesConfiguration.Apply(); // apply before creating mongo connection

                return new MongoClient(mongoConfig.Value.ConnectionString).GetDatabase(mongoConfig.Value.DatabaseName);
            })
            // .AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>))
            // .AddSingleton(typeof(IMongoReadOnlyRepository<>), typeof(MongoReadOnlyRepository<>))
            .Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));
        }

        private static IServiceCollection AddMongoDatabaseWithEventLogs(this IServiceCollection services)
        {
            // merge with upper
            return services.AddSingleton(provider =>
            {
                var mongoConfig = provider.GetRequiredService<IOptions<MongoDbSettings>>();
                EntitiesConfiguration.Apply(); // apply before creating mongo connection

                var settings = MongoClientSettings.FromConnectionString(mongoConfig.Value.ConnectionString);
                settings.ClusterConfigurator = cb =>
                {
                    cb.Subscribe<CommandStartedEvent>(e =>
                    {
                        Debug.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                    })
                    .Subscribe<CommandSucceededEvent>(e =>
                    {
                        Debug.WriteLine($"{e.CommandName} - {e.Reply.ToJson()}");
                    });
                };

                return new MongoClient(settings).GetDatabase(mongoConfig.Value.DatabaseName);
            });
        }
    }
}