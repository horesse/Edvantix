namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// Scoped implementation of <see cref="ITenantContext"/>.
/// Populated by <see cref="TenantMiddleware"/> on each incoming request.
/// </summary>
public sealed class TenantContext : ITenantContext
{
    private Guid _schoolId;

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Resolve"/> is called.</exception>
    public Guid SchoolId =>
        IsResolved
            ? _schoolId
            : throw new InvalidOperationException(
                "Tenant context not resolved. Ensure X-School-Id header is present."
            );

    /// <inheritdoc/>
    public bool IsResolved { get; private set; }

    /// <inheritdoc/>
    public void Resolve(Guid schoolId)
    {
        _schoolId = schoolId;
        IsResolved = true;
    }
}
