namespace Edvantix.Constants.Permissions;

/// <summary>
/// Permission string constants for the Scheduling service.
/// Format: service.verb-noun (kebab-case).
/// Group permissions (scheduling.create-group etc.) are NOT here — they live in
/// GroupsPermissions (Plan 03-07).
/// </summary>
public static class SchedulingPermissions
{
    /// <summary>Permission to create a new lesson slot.</summary>
    public const string CreateLessonSlot = "scheduling.create-lesson-slot";

    /// <summary>Permission to edit an existing lesson slot.</summary>
    public const string EditLessonSlot = "scheduling.edit-lesson-slot";

    /// <summary>Permission to delete a lesson slot.</summary>
    public const string DeleteLessonSlot = "scheduling.delete-lesson-slot";

    /// <summary>Permission to view the schedule.</summary>
    public const string ViewSchedule = "scheduling.view-schedule";

    /// <summary>
    /// Permission to view a teacher's own schedule.
    /// Used as a permission-based marker to distinguish teachers from students
    /// inside <c>GetScheduleQueryHandler</c> without data-driven slot inspection.
    /// Rationale: a teacher with no slots in the current date range would otherwise
    /// be misidentified as a student if detection were data-driven.
    /// </summary>
    public const string ViewOwnSchedule = "scheduling.view-own-schedule";

    /// <summary>
    /// Permission to mark or update a student's attendance for a lesson slot.
    /// Assigned to teachers and managers who are responsible for recording attendance (D-08).
    /// </summary>
    public const string MarkAttendance = "scheduling.mark-attendance";

    /// <summary>Returns all Scheduling permission strings for seeding into the permission catalogue.</summary>
    public static IReadOnlyList<string> All =>
        [CreateLessonSlot, EditLessonSlot, DeleteLessonSlot, ViewSchedule, ViewOwnSchedule, MarkAttendance];
}
