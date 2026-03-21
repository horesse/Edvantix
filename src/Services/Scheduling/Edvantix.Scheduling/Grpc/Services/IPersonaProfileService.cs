namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// Abstraction over the Persona gRPC client used to validate that a profile exists
/// before assigning a teacher or student to a lesson slot.
/// </summary>
public interface IPersonaProfileService
{
    /// <summary>
    /// Returns <see langword="true"/> if a profile with the given <paramref name="profileId"/> exists
    /// in the Persona service; <see langword="false"/> otherwise.
    /// </summary>
    /// <param name="profileId">The profile identifier to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task<bool> ValidateProfileExistsAsync(
        Guid profileId,
        CancellationToken cancellationToken = default
    );
}
