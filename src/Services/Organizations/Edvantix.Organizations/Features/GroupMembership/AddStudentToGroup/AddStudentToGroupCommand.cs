namespace Edvantix.Organizations.Features.GroupMembership.AddStudentToGroup;

/// <summary>Command to add a student (by Persona profile) to a group.</summary>
/// <param name="GroupId">The group to add the student to.</param>
/// <param name="ProfileId">The Persona profile identifier of the student.</param>
public sealed record AddStudentToGroupCommand(Guid GroupId, Guid ProfileId) : ICommand<Unit>;
