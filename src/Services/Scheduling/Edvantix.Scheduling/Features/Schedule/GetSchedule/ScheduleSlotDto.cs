namespace Edvantix.Scheduling.Features.Schedule.GetSchedule;

/// <summary>
/// Data transfer object representing a single lesson slot in the schedule response.
/// Nullable fields reflect permission-based data shaping (D-09):
/// <list type="bullet">
///   <item><description><see cref="TeacherId"/> / <see cref="TeacherName"/> — visible to manager and student; null for teacher view (teacher knows their own identity).</description></item>
///   <item><description><see cref="StudentCount"/> — visible to manager and teacher; null for student view (student does not need headcount).</description></item>
/// </list>
/// </summary>
/// <param name="Id">The lesson slot identifier.</param>
/// <param name="StartTime">Lesson start time.</param>
/// <param name="EndTime">Lesson end time.</param>
/// <param name="GroupId">Cross-service group reference (Organizations service).</param>
/// <param name="GroupName">Display name of the group, resolved from the Organizations service.</param>
/// <param name="GroupColor">Hex color of the group for UI display, resolved from the Organizations service.</param>
/// <param name="TeacherId">The assigned teacher. Null when the caller is the teacher (self-view).</param>
/// <param name="TeacherName">
/// Teacher display name. In v1 this is a placeholder derived from <see cref="TeacherId"/>;
/// full name resolution via Persona gRPC is deferred to a later plan.
/// Null when the caller is the teacher.
/// </param>
/// <param name="StudentCount">
/// Number of students in the group. Null for student callers (not relevant to them).
/// </param>
public sealed record ScheduleSlotDto(
    Guid Id,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime,
    Guid GroupId,
    string GroupName,
    string GroupColor,
    Guid? TeacherId,
    string? TeacherName,
    int? StudentCount
);
