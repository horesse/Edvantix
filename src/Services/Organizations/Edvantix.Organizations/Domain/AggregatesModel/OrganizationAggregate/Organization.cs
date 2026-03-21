namespace Edvantix.Organizations.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>
/// Represents an organization (school) — the tenant root entity.
/// Organization IS the tenant, so it does NOT implement <see cref="ITenanted"/>.
/// v1: Only <see cref="Entity.Id"/> property. Additional properties (Name, etc.) will be added in future phases.
/// </summary>
public sealed class Organization : Entity, IAggregateRoot
{
    // EF Core constructor
    private Organization() { }

    /// <summary>Initializes a new <see cref="Organization"/> with the given ID.</summary>
    /// <param name="id">The unique identifier for this organization. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is empty.</exception>
    public Organization(Guid id)
    {
        Guard.Against.Default(id, nameof(id));
        Id = id;
    }
}
