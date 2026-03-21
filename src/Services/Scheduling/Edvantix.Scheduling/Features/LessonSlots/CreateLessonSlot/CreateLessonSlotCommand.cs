namespace Edvantix.Scheduling.Features.LessonSlots.CreateLessonSlot;

/// <summary>
/// Command to create a new lesson slot, binding a group, teacher, and time range within the current tenant (school).
/// <para>
/// GroupId is a cross-service reference — the group is defined in the Organizations service (D-15).
/// The handler validates group existence via <c>IOrganizationsGroupService</c> before persisting.
/// All time values use <see cref="DateTimeOffset"/> for timezone correctness (SCH-10).
/// </para>
/// </summary>
public sealed class CreateLessonSlotCommand : ICommand<Guid>
{
    /// <summary>Cross-service reference to the group (defined in Organizations service).</summary>
    public required Guid GroupId { get; init; }

    /// <summary>The teacher assigned to this slot.</summary>
    public required Guid TeacherId { get; init; }

    /// <summary>Lesson start time (inclusive). Must be before <see cref="EndTime"/>.</summary>
    public required DateTimeOffset StartTime { get; init; }

    /// <summary>Lesson end time (exclusive). Must be after <see cref="StartTime"/>.</summary>
    public required DateTimeOffset EndTime { get; init; }
}
