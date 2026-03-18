using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Organizations.Infrastructure;

/// <summary>
/// Design-time factory for EF Core tooling (migrations, scaffolding).
/// Uses a stub <see cref="TenantContext"/> so that the tenant query filters can be built
/// without a live HTTP request context.
/// </summary>
public class OrganizationsDbContextFactory : IDesignTimeDbContextFactory<OrganizationsDbContext>
{
    /// <inheritdoc/>
    public OrganizationsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<OrganizationsDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Organizations"))
            .UseSnakeCaseNamingConvention();

        // Design-time stub: TenantContext is not resolved, but EF tooling only needs
        // the model — it never executes queries against the filter.
        return new OrganizationsDbContext(optionsBuilder.Options, new TenantContext());
    }
}
