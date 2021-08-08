namespace Playground.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;

    public class IntegrationEventLogDbContextDesignTimeFactory : IDesignTimeDbContextFactory<IntegrationEventLogDbContext>
    {
        public IntegrationEventLogDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogDbContext>();

            optionsBuilder.UseSqlServer(".", options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

            return new IntegrationEventLogDbContext(optionsBuilder.Options);
        }
    }
}