namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// Represents the current tenant (school) context for the request.
/// Resolved by <see cref="TenantMiddleware"/> from the <c>X-School-Id</c> HTTP header.
/// </summary>
public interface ITenantContext
{
    /// <summary>Gets the resolved school identifier for the current request.</summary>
    /// <exception cref="InvalidOperationException">Thrown when the context has not been resolved.</exception>
    Guid SchoolId { get; }

    /// <summary>Gets a value indicating whether the tenant context has been resolved.</summary>
    bool IsResolved { get; }

    /// <summary>Resolves the tenant context with the given school identifier.</summary>
    /// <param name="schoolId">The school identifier extracted from the request header.</param>
    void Resolve(Guid schoolId);
}
