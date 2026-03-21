namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// Abstraction over the Organizations service used to validate that a group exists
/// before creating a lesson slot.
/// Cross-service reference: Groups live in the Organizations service (D-15);
/// Scheduling stores only the GroupId as a plain <see cref="Guid"/>.
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
}
