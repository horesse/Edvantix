using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.EntityHub.Infrastructure;

public class DataVaultContextFactory : IDesignTimeDbContextFactory<EntityHubContext>
{
    public EntityHubContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<EntityHubContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("EntityHub"))
            .UseSnakeCaseNamingConvention();
        return new EntityHubContext(optionsBuilder.Options);
    }
}
