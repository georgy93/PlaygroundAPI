namespace Playground.Persistence;

using Application.Common;
using Application.Common.Integration;
using Domain.Entities.Aggregates.BuyerAggregate;
using Domain.Entities.Aggregates.OrderAggregate;
using EntityFramework;
using EntityFramework.Repositories;
using EntityFramework.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mongo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using Playground.Persistence.EntityFramework.DbContextInterceptors;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {

        public IServiceCollection AddPersistence(IConfiguration config) => services
            .AddEntityFramework(config)
            //.AddMongoDatabase(config)
            .AddDataStoresHealthChecks();

        private IServiceCollection AddEntityFramework(IConfiguration config)
        {
            // https://github.com/dotnet/aspnetcore/issues/17093
            services
                 .AddDbContext<IntegrationEventLogDbContext>(contextBuilder =>
                 {
                     contextBuilder.UseSqlServer(
                         connectionString: config.GetConnectionString("DefaultConnection"),
                         sqlServerOptionsAction: sqlServerContextBuilder =>
                         {
                             sqlServerContextBuilder.MigrationsAssembly(typeof(IntegrationEventLogDbContext).Assembly.GetName().Name);
                             //sqlServerContextBuilder.EnableRetryOnFailure(maxRetryCount: 3, maxRetryDelay: TimeSpan.FromSeconds(3), errorNumbersToAdd: null);
                         });
                 }, contextLifetime: ServiceLifetime.Scoped) //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                 .AddDbContext<AppDbContext>((sp, contextBuilder) =>
                 {
                     contextBuilder
                     .UseSqlServer(
                         connectionString: config.GetConnectionString("DefaultConnection"),
                         sqlServerOptionsAction: sqlServerContextBuilder =>
                         {
                             sqlServerContextBuilder.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name);
                             // sqlServerContextBuilder.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                         })
                     .AddInterceptors(
                         sp.GetRequiredService<AuditableEntitiesInterceptor>(),
                         sp.GetRequiredService<ValidateEntitiesStateInterceptor>(),
                         sp.GetRequiredService<IncreaseAggregatesVersionInterceptor>());
                 }, contextLifetime: ServiceLifetime.Scoped) // Showing explicitly that the DbContext is shared across the HTTP request scope(graph of objects started in the HTTP request))
                 .AddIdentity<ApplicationUser, IdentityRole>()
                 .AddRoles<IdentityRole>()
                 .AddEntityFrameworkStores<AppDbContext>();

            return services
                .AddScoped<IIntegrationEventLogService, IntegrationEventLogService>()
                .AddScoped<AuditableEntitiesInterceptor>()
                .AddScoped<ValidateEntitiesStateInterceptor>()
                .AddScoped<IncreaseAggregatesVersionInterceptor>()
                .AddScoped<IOrderRepository, OrderRepository>()
                .AddScoped<IBuyerRepository, BuyerRepository>();
        }

        // https://www.youtube.com/watch?v=bdgtYbGYsK0
        private IServiceCollection AddDataStoresHealthChecks()
        {
            services
                .AddHealthChecks()
                .AddDbContextCheck<AppDbContext>()
                .AddDbContextCheck<IntegrationEventLogDbContext>();

            return services;
        }

        private IServiceCollection AddMongoDatabase(IConfiguration config)
        {
            var mongoConnectionString = config.GetConnectionString("MongoConnection");

            services
                .AddHealthChecks()
                .AddMongoDb(timeout: TimeSpan.FromSeconds(3));

            return services.AddSingleton(provider =>
            {
                EntitiesConfigurator.Apply(); // apply before creating mongo connection

                var mongoConfig = provider.GetRequiredService<IOptions<MongoDbSettings>>();
                var settings = MongoClientSettings.FromConnectionString(mongoConnectionString);

                if (mongoConfig.Value.SubscribeToEvents)
                {
                    var logger = provider.GetRequiredService<ILogger<MongoClient>>();

                    settings.ClusterConfigurator = clusterBuilder =>  // TODO: clusterBuilder.ConfigureCluster for transactions or settings.ConnectionMode ConnectionMode
                        clusterBuilder
                        .Subscribe<CommandStartedEvent>(e => logger.LogInformation("{CommandName} - {Command}", e.CommandName, e.Command.ToJson()))
                        .Subscribe<CommandSucceededEvent>(e => logger.LogInformation("{CommandName} - {Reply}", e.CommandName, e.Reply.ToJson()))
                        .Subscribe<CommandFailedEvent>(e => logger.LogError("{CommandName} - {Failure}", e.CommandName, e.Failure.ToJson()));
                }

                return new MongoClient(settings).GetDatabase(mongoConfig.Value.DatabaseName);
            })
            // .AddSingleton(typeof(IMongoRepository<>), typeof(MongoRepository<>))
            // .AddSingleton(typeof(IMongoReadOnlyRepository<>), typeof(MongoReadOnlyRepository<>))
            .Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));
        }
    }
}