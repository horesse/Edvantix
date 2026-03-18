namespace Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;

/// <summary>
/// Represents a named permission in the global catalogue.
/// Permissions are NOT tenant-scoped — they are shared across all schools and therefore
/// have no HasQueryFilter in <c>OrganizationsDbContext</c>.
/// </summary>
public sealed class Permission : Entity, IAggregateRoot
{
    /// <summary>Gets the permission name (e.g., "scheduling:read").</summary>
    public string Name { get; private set; } = string.Empty;

    // EF Core constructor
    private Permission() { }

    /// <summary>Initializes a new global <see cref="Permission"/>.</summary>
    /// <param name="name">The permission name. Must not be null or whitespace.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public Permission(string name)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Name = name.Trim();
    }
}
