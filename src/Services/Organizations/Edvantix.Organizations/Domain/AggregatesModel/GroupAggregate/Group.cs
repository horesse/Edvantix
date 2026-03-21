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
}
