namespace Edvantix.Organizations.Domain.Abstractions;

/// <summary>
/// Marks a domain entity as tenant-scoped (belonging to a specific school).
/// All entities implementing this interface must have a corresponding HasQueryFilter
/// in <c>OrganizationsDbContext</c> that filters by <see cref="SchoolId"/>.
/// </summary>
public interface ITenanted
{
    /// <summary>Gets the school identifier that owns this entity.</summary>
    Guid SchoolId { get; }
}
