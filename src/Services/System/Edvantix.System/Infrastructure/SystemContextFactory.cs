using Edvantix.Constants.Aspire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.System.Infrastructure;

public class SystemContextFactory : IDesignTimeDbContextFactory<SystemContext>
{
    public SystemContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SystemContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("System"))
            .UseSnakeCaseNamingConvention();
        return new SystemContext(optionsBuilder.Options);
    }
}
