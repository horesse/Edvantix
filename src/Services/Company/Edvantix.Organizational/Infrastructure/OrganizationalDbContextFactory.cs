using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Organizational.Infrastructure;

public class OrganizationalDbContextFactory : IDesignTimeDbContextFactory<OrganizationalDbContext>
{
    public OrganizationalDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OrganizationalDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Organization"))
            .UseSnakeCaseNamingConvention();
        return new OrganizationalDbContext(optionsBuilder.Options);
    }
}
