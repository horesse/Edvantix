namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// DTO carrying the group display information needed when shaping <c>ScheduleSlotDto</c> responses.
/// Populated from the Organizations service via <see cref="IOrganizationsGroupService.GetGroupAsync"/>.
/// </summary>
public sealed record GroupInfoDto(string Name, string Color);

/// <summary>
/// Abstraction over the Organizations service used to validate and resolve group data
/// for the Scheduling service.
/// Cross-service reference: Groups live in the Organizations service (D-15);
/// Scheduling stores only GroupId as a plain <see cref="Guid"/>.
/// </summary>
public interface IOrganizationsGroupService
{
    /// <summary>
    /// Returns <see langword="true"/> if a group with the given <paramref name="groupId"/> exists
    /// in the Organizations service; <see langword="false"/> otherwise.
    /// </summary>
    /// <param name="groupId">The group identifier to validate.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    Task<bool> GroupExistsAsync(Guid groupId, CancellationToken ct);

    /// <summary>
    /// Returns the list of group IDs that the given student (identified by <paramref name="profileId"/>)
    /// belongs to within the specified school.
    /// Called during schedule queries to filter lesson slots to only the groups the student is a member of (SCH-05).
    /// </summary>
    /// <param name="schoolId">The tenant school to scope the membership lookup.</param>
    /// <param name="profileId">The student's profile identifier.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    Task<List<Guid>> GetGroupsForStudentAsync(Guid schoolId, Guid profileId, CancellationToken ct);

    /// <summary>
    /// Returns display information (name, color) for the given group, or <see langword="null"/>
    /// if the group does not exist in the Organizations service.
    /// Used to populate <c>GroupName</c> and <c>GroupColor</c> fields in <c>ScheduleSlotDto</c>.
    /// </summary>
    /// <param name="groupId">The group identifier to look up.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    Task<GroupInfoDto?> GetGroupAsync(Guid groupId, CancellationToken ct);
}
