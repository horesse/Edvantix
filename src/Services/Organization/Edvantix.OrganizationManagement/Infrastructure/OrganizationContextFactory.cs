using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.OrganizationManagement.Infrastructure;

public class OrganizationContextFactory : IDesignTimeDbContextFactory<OrganizationContext>
{
    public OrganizationContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OrganizationContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Organization"))
            .UseSnakeCaseNamingConvention();
        return new OrganizationContext(optionsBuilder.Options);
    }
}
