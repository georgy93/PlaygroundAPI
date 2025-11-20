namespace Playground.Persistence.EntityFramework;

using Application.Common.Integration;

/// <summary>
/// PM Console: Add-Migration InitialCreate -Context IntegrationEventLogDbContext -OutputDir EntityFramework\Migrations\IntegrationEventLogDbContextMigrations
/// https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/providers?tabs=vs
/// </summary>
public class IntegrationEventLogDbContext : DbContext
{
    public const string DEFAULT_SCHEMA = "integration";

    public IntegrationEventLogDbContext(DbContextOptions<IntegrationEventLogDbContext> options) : base(options)
    { }

    public DbSet<IntegrationEventLogEntry> IntegrationEventEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<IntegrationEventLogEntry>(ConfigureIntegrationEventLogEntry);
    }

    static void ConfigureIntegrationEventLogEntry(EntityTypeBuilder<IntegrationEventLogEntry> builder)
    {
        builder.ToTable("IntegrationEventLog", DEFAULT_SCHEMA);

        builder.HasKey(e => e.EventId);

        builder.Property(e => e.EventId)
            .IsRequired();

        builder.Property(e => e.MessageJsonContent)
            .IsRequired();

        builder.Property(e => e.CreationTime)
            .IsRequired();

        builder.Property(e => e.PublishState)
            .IsRequired();

        builder.Property(e => e.TimesSent)
            .IsRequired();

        builder
            .Property(m => m.Type)
            .IsRequired()
            .HasConversion(
                t => t.AssemblyQualifiedName,
                t => Type.GetType(t));

        builder.Ignore(t => t.IntegrationEvent);
    }
}
