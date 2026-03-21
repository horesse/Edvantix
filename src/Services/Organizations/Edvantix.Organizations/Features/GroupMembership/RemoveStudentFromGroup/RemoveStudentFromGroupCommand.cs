namespace Edvantix.Organizations.Features.GroupMembership.RemoveStudentFromGroup;

/// <summary>Command to remove a student from a group.</summary>
/// <param name="GroupId">The group to remove the student from.</param>
/// <param name="ProfileId">The Persona profile identifier of the student to remove.</param>
public sealed record RemoveStudentFromGroupCommand(Guid GroupId, Guid ProfileId) : ICommand<Unit>;
