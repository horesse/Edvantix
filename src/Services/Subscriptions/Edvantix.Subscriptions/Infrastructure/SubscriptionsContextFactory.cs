using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Edvantix.Subscriptions.Infrastructure;

/// <summary>
/// Design-time factory for creating SubscriptionsContext instances.
/// Used by EF Core tools for migrations.
/// </summary>
public class SubscriptionsContextFactory : IDesignTimeDbContextFactory<SubscriptionsContext>
{
    /// <summary>
    /// Creates a new instance of the SubscriptionsContext for design-time operations.
    /// </summary>
    /// <param name="args">Arguments passed by the EF Core tools.</param>
    /// <returns>A configured SubscriptionsContext instance.</returns>
    public SubscriptionsContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddInMemoryCollection(
                [new KeyValuePair<string, string?>("Identity:Realm", "design-time")]!
            )
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SubscriptionsContext>();
        optionsBuilder
            .UseNpgsql(configuration.GetConnectionString("Subscription"))
            .UseSnakeCaseNamingConvention();
        return new SubscriptionsContext(optionsBuilder.Options);
    }
}
