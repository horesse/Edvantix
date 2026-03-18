using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// Extension methods for wiring tenant context into the DI container and HTTP pipeline.
/// </summary>
public static class TenantServiceExtensions
{
    /// <summary>
    /// Registers <see cref="ITenantContext"/> as a scoped service backed by <see cref="TenantContext"/>.
    /// </summary>
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();
        return services;
    }

    /// <summary>
    /// Adds <see cref="TenantMiddleware"/> to the pipeline.
    /// Call this before <c>UseAuthorization()</c> so that the tenant context is available
    /// in authorization handlers.
    /// </summary>
    public static IApplicationBuilder UseTenantContext(this IApplicationBuilder app)
    {
        app.UseMiddleware<TenantMiddleware>();
        return app;
    }
}
