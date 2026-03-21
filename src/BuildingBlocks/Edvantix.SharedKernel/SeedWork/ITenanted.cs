namespace Edvantix.SharedKernel.SeedWork;

/// <summary>
/// Marks a domain entity as tenant-scoped (belonging to a specific school).
/// All entities implementing this interface must have a corresponding HasQueryFilter
/// in their service's DbContext that filters by <see cref="SchoolId"/>.
/// Moved to SharedKernel (from Organizations) so multiple services can implement
/// tenant isolation without taking a dependency on the Organizations assembly.
/// </summary>
public interface ITenanted
{
    /// <summary>Gets the school identifier that owns this entity.</summary>
    Guid SchoolId { get; }
}
