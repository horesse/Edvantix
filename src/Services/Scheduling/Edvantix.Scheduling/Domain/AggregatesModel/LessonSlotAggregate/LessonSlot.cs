namespace Edvantix.Scheduling.Domain.AggregatesModel.LessonSlotAggregate;

/// <summary>
/// Represents a scheduled lesson slot within a school (tenant).
/// A lesson slot binds a group (from the Organizations service), a teacher, and a time range.
/// GroupId is a plain <see cref="Guid"/> — Group lives in the Organizations service (D-15),
/// so there is no navigation property to Group in this aggregate.
/// Lesson slots are hard-deleted (no soft-delete), so only a single tenant HasQueryFilter is needed.
/// </summary>
public sealed class LessonSlot : Entity, IAggregateRoot, ITenanted
{
    /// <summary>Gets the school (tenant) that owns this lesson slot.</summary>
    public Guid SchoolId { get; private set; }

    /// <summary>
    /// Gets the group this lesson is for.
    /// Plain Guid cross-service reference — Group is defined in the Organizations service (D-15).
    /// No navigation property or foreign key constraint to a local Group entity.
    /// </summary>
    public Guid GroupId { get; private set; }

    /// <summary>Gets the teacher assigned to this lesson slot.</summary>
    public Guid TeacherId { get; private set; }

    /// <summary>Gets the lesson start time (inclusive). Uses DateTimeOffset for timezone correctness (SCH-10).</summary>
    public DateTimeOffset StartTime { get; private set; }

    /// <summary>Gets the lesson end time (exclusive). Uses DateTimeOffset for timezone correctness (SCH-10).</summary>
    public DateTimeOffset EndTime { get; private set; }

    // EF Core parameterless constructor
    private LessonSlot() { }

    /// <summary>
    /// Creates a new <see cref="LessonSlot"/> for the given school.
    /// </summary>
    /// <param name="schoolId">The tenant school. Must not be empty.</param>
    /// <param name="groupId">Cross-service reference to the group (Organizations service). Must not be empty.</param>
    /// <param name="teacherId">The assigned teacher. Must not be empty.</param>
    /// <param name="startTime">Lesson start time. Must be before <paramref name="endTime"/>.</param>
    /// <param name="endTime">Lesson end time. Must be strictly after <paramref name="startTime"/>.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when any GUID is default or when <paramref name="endTime"/> is not after <paramref name="startTime"/>.
    /// </exception>
    public LessonSlot(
        Guid schoolId,
        Guid groupId,
        Guid teacherId,
        DateTimeOffset startTime,
        DateTimeOffset endTime
    )
    {
        Guard.Against.Default(schoolId, nameof(schoolId));
        Guard.Against.Default(groupId, nameof(groupId));
        Guard.Against.Default(teacherId, nameof(teacherId));

        if (endTime <= startTime)
        {
            throw new ArgumentException("EndTime must be strictly after StartTime.", nameof(endTime));
        }

        SchoolId = schoolId;
        GroupId = groupId;
        TeacherId = teacherId;
        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Reschedules this lesson slot to a new time range.
    /// </summary>
    /// <param name="newStart">New start time.</param>
    /// <param name="newEnd">New end time. Must be strictly after <paramref name="newStart"/>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newEnd"/> is not after <paramref name="newStart"/>.</exception>
    public void Reschedule(DateTimeOffset newStart, DateTimeOffset newEnd)
    {
        if (newEnd <= newStart)
        {
            throw new ArgumentException("EndTime must be strictly after StartTime.", nameof(newEnd));
        }

        StartTime = newStart;
        EndTime = newEnd;
    }

    /// <summary>
    /// Assigns a different teacher to this lesson slot.
    /// </summary>
    /// <param name="newTeacherId">The replacement teacher. Must not be empty.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="newTeacherId"/> is the default Guid.</exception>
    public void ChangeTeacher(Guid newTeacherId)
    {
        Guard.Against.Default(newTeacherId, nameof(newTeacherId));
        TeacherId = newTeacherId;
    }
}
