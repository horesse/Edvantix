namespace Edvantix.Scheduling.Features.LessonSlots.EditLessonSlot;

/// <summary>
/// Command to edit an existing lesson slot's teacher or time range.
/// <para>
/// The conflict check excludes the current slot itself (D-06), allowing a slot to be
/// rescheduled to its own time without a false conflict. All time values use
/// <see cref="DateTimeOffset"/> for timezone correctness (SCH-10).
/// </para>
/// </summary>
public sealed class EditLessonSlotCommand : ICommand<Unit>
{
    /// <summary>The identifier of the lesson slot to edit.</summary>
    public required Guid Id { get; init; }

    /// <summary>The new teacher for this slot (may be the same as the current one).</summary>
    public required Guid TeacherId { get; init; }

    /// <summary>New start time (inclusive). Must be before <see cref="EndTime"/>.</summary>
    public required DateTimeOffset StartTime { get; init; }

    /// <summary>New end time (exclusive). Must be after <see cref="StartTime"/>.</summary>
    public required DateTimeOffset EndTime { get; init; }
}
