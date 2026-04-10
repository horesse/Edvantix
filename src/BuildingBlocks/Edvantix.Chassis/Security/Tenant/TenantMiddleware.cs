using Microsoft.AspNetCore.Http;

namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// Middleware для ASP.NET Core, который извлекает заголовок <c>X-Organization-Id</c> из входящего
/// запроса и определяет scoped-контекст <see cref="ITenantContext"/>.
/// Регистрируется через <see cref="TenantServiceExtensions.UseTenantContext"/>.
/// </summary>
public sealed class TenantMiddleware(RequestDelegate next)
{
    private const string SchoolIdHeader = "X-Organization-Id";

    /// <summary>
    /// Вызывает middleware.
    /// </summary>
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
