namespace Edvantix.Scheduling.Features.Schedule.GetSchedule;

/// <summary>
/// Query that returns the schedule for the current tenant within a date range.
/// Results are filtered based on the caller's permission level (D-07, D-08):
/// <list type="bullet">
///   <item><description>Manager (<c>scheduling.create-lesson-slot</c>) — all tenant slots.</description></item>
///   <item><description>Teacher (<c>scheduling.view-own-schedule</c>) — only the caller's own slots.</description></item>
///   <item><description>Student (neither manager nor teacher) — only slots for groups the student belongs to, resolved via Organizations gRPC.</description></item>
/// </list>
/// All time values use <see cref="DateTimeOffset"/> for timezone correctness (SCH-10).
/// </summary>
/// <param name="DateFrom">Inclusive start of the requested date range.</param>
/// <param name="DateTo">Inclusive end of the requested date range.</param>
public sealed record GetScheduleQuery(DateTimeOffset DateFrom, DateTimeOffset DateTo)
    : IQuery<List<ScheduleSlotDto>>;
