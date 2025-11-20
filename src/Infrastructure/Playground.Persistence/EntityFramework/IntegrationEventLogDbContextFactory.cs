namespace Playground.Persistence.EntityFramework;

public class IntegrationEventLogDbContextFactory : IDesignTimeDbContextFactory<IntegrationEventLogDbContext>
{
    public IntegrationEventLogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IntegrationEventLogDbContext>();

        optionsBuilder.UseSqlServer(".", options => options.MigrationsAssembly(GetType().Assembly.GetName().Name));

        return new IntegrationEventLogDbContext(optionsBuilder.Options);
    }
}