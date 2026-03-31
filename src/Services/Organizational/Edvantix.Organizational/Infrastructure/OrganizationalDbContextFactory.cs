using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Organizational.Infrastructure;

/// <summary>Фабрика для создания миграций через EF Core CLI (design-time).</summary>
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
            .UseNpgsql(configuration.GetConnectionString("Organizational"))
            .UseSnakeCaseNamingConvention();

        return new OrganizationalDbContext(optionsBuilder.Options);
    }
}
