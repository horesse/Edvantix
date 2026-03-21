using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Scheduling.Infrastructure;

/// <summary>
/// Design-time factory for EF Core tooling (migrations, scaffolding).
/// Uses a stub <see cref="TenantContext"/> so that the tenant query filters can be built
/// without a live HTTP request context.
/// </summary>
public class SchedulingDbContextFactory : IDesignTimeDbContextFactory<SchedulingDbContext>
{
    /// <inheritdoc/>
    public SchedulingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SchedulingDbContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Scheduling"))
            .UseSnakeCaseNamingConvention();

        // Design-time stub: TenantContext is not resolved, but EF tooling only needs
        // the model — it never executes queries against the filter.
        return new SchedulingDbContext(optionsBuilder.Options, new TenantContext());
    }
}
