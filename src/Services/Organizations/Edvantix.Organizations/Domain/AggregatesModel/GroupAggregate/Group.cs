namespace Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

/// <summary>
/// Represents a student group within a school (tenant).
/// Groups are soft-deletable and tenant-scoped — both filters are combined into a single
/// HasQueryFilter in <c>OrganizationsDbContext</c> because EF Core only supports one
/// HasQueryFilter per entity type.
/// </summary>
public sealed class Group : Entity, IAggregateRoot, ISoftDelete, ITenanted
{
    /// <summary>Gets the display name of the group.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Gets the school this group belongs to.</summary>
    public Guid SchoolId { get; private set; }

    /// <summary>Gets the maximum number of students allowed in this group.</summary>
    public int MaxCapacity { get; private set; }

    /// <summary>Gets the color identifier used for UI display (e.g., a hex color string).</summary>
    public string Color { get; private set; } = string.Empty;

    /// <inheritdoc/>
    public bool IsDeleted { get; set; }

    // Backing field used by EF Core property access mode to populate Members without exposing Add/Remove
    private readonly List<GroupMembership> _members = [];

    /// <summary>Gets the current members enrolled in this group.</summary>
    public IReadOnlyCollection<GroupMembership> Members => _members.AsReadOnly();

    // EF Core constructor
    private Group() { }

    /// <summary>Initializes a new <see cref="Group"/> for the given school.</summary>
    /// <param name="name">Display name for the group. Must not be null or whitespace.</param>
    /// <param name="schoolId">The school this group belongs to. Must not be empty.</param>
    /// <param name="maxCapacity">Maximum student count for this group.</param>
    /// <param name="color">UI color identifier (e.g., hex code).</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="name"/> is null/whitespace or <paramref name="schoolId"/> is empty.
    /// </exception>
    public Group(string name, Guid schoolId, int maxCapacity, string color)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));
        Guard.Against.Default(schoolId, nameof(schoolId));

        Name = name.Trim();
        SchoolId = schoolId;
        MaxCapacity = maxCapacity;
        Color = color;
    }

    /// <summary>Updates the group's name, max capacity, and color.</summary>
    /// <param name="name">The new display name. Must not be null or whitespace.</param>
    /// <param name="maxCapacity">The new maximum student count.</param>
    /// <param name="color">The new UI color identifier.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public void Update(string name, int maxCapacity, string color)
    {
        Guard.Against.NullOrWhiteSpace(name, nameof(name));

        Name = name.Trim();
        MaxCapacity = maxCapacity;
        Color = color;
    }

    /// <inheritdoc/>
    public void Delete() => IsDeleted = true;

    /// <summary>
    /// Adds a student to this group. Idempotent — if the student is already a member, this is a no-op.
    /// Per SCH-06, duplicate membership must be silently ignored without error or duplicate rows.
    /// </summary>
    /// <param name="profileId">The Persona profile identifier of the student to add. Must not be empty.</param>
    /// <param name="addedAt">The timestamp of membership creation (typically UTC now).</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="profileId"/> is empty.</exception>
    public void AddMember(Guid profileId, DateTimeOffset addedAt)
    {
        Guard.Against.Default(profileId, nameof(profileId));

        // Idempotent: silently ignore if the student is already a member.
        if (_members.Any(m => m.ProfileId == profileId))
        {
            return;
        }

        _members.Add(new GroupMembership(Id, profileId, SchoolId, addedAt));
    }

    /// <summary>
    /// Removes a student from this group. No-op if the student is not a member.
    /// </summary>
    /// <param name="profileId">The Persona profile identifier of the student to remove.</param>
    public void RemoveMember(Guid profileId)
    {
        var existing = _members.FirstOrDefault(m => m.ProfileId == profileId);

        if (existing is not null)
        {
            _members.Remove(existing);
        }
    }
}
