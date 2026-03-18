using Microsoft.AspNetCore.Http;

namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// ASP.NET Core middleware that extracts the <c>X-School-Id</c> header from the incoming
/// request and resolves the scoped <see cref="ITenantContext"/>.
/// Register via <see cref="TenantServiceExtensions.UseTenantContext"/>.
/// </summary>
public sealed class TenantMiddleware(RequestDelegate next)
{
    private const string SchoolIdHeader = "X-School-Id";

    /// <summary>Invokes the middleware.</summary>
    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (
            context.Request.Headers.TryGetValue(SchoolIdHeader, out var value)
            && Guid.TryParse(value, out var schoolId)
        )
        {
            tenantContext.Resolve(schoolId);
        }

        await next(context);
    }
}
