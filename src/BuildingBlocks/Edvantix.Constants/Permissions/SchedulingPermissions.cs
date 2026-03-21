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

    /// <summary>Returns all Scheduling permission strings for seeding into the permission catalogue.</summary>
    public static IReadOnlyList<string> All =>
        [CreateLessonSlot, EditLessonSlot, DeleteLessonSlot, ViewSchedule];
}
