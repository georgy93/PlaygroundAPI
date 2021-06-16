namespace Playground.Persistence.EntityFramework.Extensions
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public static class MigrationsExtensions
    {
        public static async Task MigrateDataBaseAsync<TDbContext>(this IHost host, Func<TDbContext, IServiceProvider, Task> seedAsync = null)
            where TDbContext : DbContext
        {
            using var serviceScope = host.Services.CreateScope();

            var services = serviceScope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<TDbContext>>();
            var dbContext = services.GetRequiredService<TDbContext>();
            var dbContextName = typeof(TDbContext).Name;

            try
            {
                logger.LogInformation($"Migrating database associated with context {dbContextName}");

                await dbContext.Database.MigrateAsync();

                if (seedAsync != null)
                    await seedAsync(dbContext, services);

                logger.LogInformation($"Migrated database associated with context {dbContextName}");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"An error occurred while migrating the database used on context {dbContextName}");
                throw;
            }
        }
    }
}