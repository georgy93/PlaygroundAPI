namespace Playground.Persistence.EntityFramework
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;
    using Utils.Helpers;

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var configuration = ConfigurationHelper.BuildConfigurationRoot(args);
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}