namespace Edvantix.Subscriptions.Infrastructure;

/// <summary>
/// Extension methods for configuring persistence services.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds persistence services including database context and repositories.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    public static void AddPersistenceServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddAzurePostgresDbContext<SubscriptionsContext>(
            Components.Database.Subscription,
            app =>
            {
                if (app.Environment.IsDevelopment())
                {
                    services.AddMigration<SubscriptionsContext>();
                }
                else
                {
                    services.AddMigration<SubscriptionsContext>();
                }

                services.AddRepositories(typeof(ISubscriptionsApiMarker));
            }
        );
    }
}
