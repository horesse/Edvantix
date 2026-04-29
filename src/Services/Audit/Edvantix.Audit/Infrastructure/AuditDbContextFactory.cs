using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Audit.Infrastructure;

public class AuditDbContextFactory : IDesignTimeDbContextFactory<AuditDbContext>
{
    public AuditDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<AuditDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Audit"))
            .UseSnakeCaseNamingConvention();
        return new AuditDbContext(optionsBuilder.Options);
    }
}
