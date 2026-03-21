namespace Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Represents a student's membership in a group. Owned by the Group aggregate.
/// Tenant-scoped via <see cref="SchoolId"/> with a query filter in <c>OrganizationsDbContext</c>.
/// </summary>
public sealed class GroupMembership : Entity, ITenanted
{
    /// <summary>Gets the group this membership belongs to.</summary>
    public Guid GroupId { get; private set; }

    /// <summary>Gets the Persona profile identifier of the enrolled student.</summary>
    public Guid ProfileId { get; private set; }

    /// <summary>Gets the school (tenant) this membership is scoped to.</summary>
    public Guid SchoolId { get; private set; }

    /// <summary>Gets the timestamp when the student was added to the group.</summary>
    public DateTimeOffset AddedAt { get; private set; }

    // EF Core constructor
    private GroupMembership() { }

    /// <summary>Initializes a new <see cref="GroupMembership"/> for a student within a group.</summary>
    /// <param name="groupId">The group identifier. Must not be empty.</param>
    /// <param name="profileId">The student's Persona profile identifier. Must not be empty.</param>
    /// <param name="schoolId">The tenant (school) identifier. Must not be empty.</param>
    /// <param name="addedAt">The timestamp the membership was created.</param>
    /// <exception cref="ArgumentException">Thrown when any guid parameter is empty.</exception>
    public GroupMembership(Guid groupId, Guid profileId, Guid schoolId, DateTimeOffset addedAt)
    {
        Guard.Against.Default(groupId, nameof(groupId));
        Guard.Against.Default(profileId, nameof(profileId));
        Guard.Against.Default(schoolId, nameof(schoolId));

        GroupId = groupId;
        ProfileId = profileId;
        SchoolId = schoolId;
        AddedAt = addedAt;
    }
}
