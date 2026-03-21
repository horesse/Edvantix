using Edvantix.Scheduling.Grpc.Services;

namespace Edvantix.Scheduling.Features.Schedule.GetSchedule;

/// <summary>
/// Mapping context that bundles a <see cref="LessonSlot"/> with the display metadata needed
/// to shape a <see cref="ScheduleSlotDto"/> for the caller's permission level.
/// </summary>
public sealed record ScheduleSlotMappingContext(
    LessonSlot Slot,
    GroupInfoDto GroupInfo,
    bool IsManager,
    bool IsTeacher
);

/// <summary>
/// Maps a <see cref="ScheduleSlotMappingContext"/> to a <see cref="ScheduleSlotDto"/>,
/// applying permission-based field shaping (D-09):
/// <list type="bullet">
///   <item><description>Manager — sees TeacherId, TeacherName, and StudentCount.</description></item>
///   <item><description>Teacher — TeacherId/TeacherName hidden (self-view); StudentCount visible.</description></item>
///   <item><description>Student — sees TeacherId/TeacherName; StudentCount hidden.</description></item>
/// </list>
/// </summary>
public sealed class ScheduleSlotMapper : Mapper<ScheduleSlotMappingContext, ScheduleSlotDto>
{
    /// <inheritdoc/>
    public override ScheduleSlotDto Map(ScheduleSlotMappingContext source) =>
        source switch
        {
            { IsManager: true } => new ScheduleSlotDto(
                Id: source.Slot.Id,
                StartTime: source.Slot.StartTime,
                EndTime: source.Slot.EndTime,
                GroupId: source.Slot.GroupId,
                GroupName: source.GroupInfo.Name,
                GroupColor: source.GroupInfo.Color,
                // Manager sees teacher identity and headcount.
                TeacherId: source.Slot.TeacherId,
                // v1 placeholder — full name resolution via Persona gRPC deferred to later plan.
                TeacherName: source.Slot.TeacherId.ToString(),
                // v1 placeholder — actual student count requires attendance data (Phase 4).
                StudentCount: 0
            ),
            { IsTeacher: true } => new ScheduleSlotDto(
                Id: source.Slot.Id,
                StartTime: source.Slot.StartTime,
                EndTime: source.Slot.EndTime,
                GroupId: source.Slot.GroupId,
                GroupName: source.GroupInfo.Name,
                GroupColor: source.GroupInfo.Color,
                // Teacher does not need their own ID echoed back.
                TeacherId: null,
                TeacherName: null,
                // v1 placeholder for student count.
                StudentCount: 0
            ),
            _ => new ScheduleSlotDto(
                Id: source.Slot.Id,
                StartTime: source.Slot.StartTime,
                EndTime: source.Slot.EndTime,
                GroupId: source.Slot.GroupId,
                GroupName: source.GroupInfo.Name,
                GroupColor: source.GroupInfo.Color,
                // Student sees teacher identity (to know who is teaching).
                TeacherId: source.Slot.TeacherId,
                TeacherName: source.Slot.TeacherId.ToString(),
                // Student does not need group headcount.
                StudentCount: null
            ),
        };
}
