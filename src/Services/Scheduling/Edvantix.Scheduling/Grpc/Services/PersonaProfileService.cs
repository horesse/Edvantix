using Edvantix.Persona.Grpc.Services;
using Grpc.Core;

namespace Edvantix.Scheduling.Grpc.Services;

/// <summary>
/// Wraps the Persona <see cref="ProfileGrpcService.ProfileGrpcServiceClient"/> to expose a
/// domain-friendly validation method used by scheduling commands (teacher/student assignment).
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class PersonaProfileService(ProfileGrpcService.ProfileGrpcServiceClient client)
    : IPersonaProfileService
{
    /// <inheritdoc/>
    public async Task<bool> ValidateProfileExistsAsync(
        Guid profileId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var request = new GetProfileRequest { ProfileId = profileId.ToString() };
            var reply = await client.GetProfileAsync(request, cancellationToken: cancellationToken);

            // A non-empty id in the reply confirms the profile exists
            return !string.IsNullOrWhiteSpace(reply.Id);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return false;
        }
    }
}
