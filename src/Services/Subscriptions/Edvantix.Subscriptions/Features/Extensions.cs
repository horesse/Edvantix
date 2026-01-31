namespace Edvantix.Subscriptions.Features;

/// <summary>
/// Extension methods for registering API features.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Adds all API features for the Subscriptions microservice.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public static void AddApiFeature(this IServiceCollection services)
    {
        // Features will be registered here as they are implemented:
        // - Plans feature (CRUD for subscription plans)
        // - Subscriptions feature (organization subscriptions management)
        // - Usage tracking feature (limit usage monitoring)
        // - Limits feature (limit checking and enforcement)
    }
}
