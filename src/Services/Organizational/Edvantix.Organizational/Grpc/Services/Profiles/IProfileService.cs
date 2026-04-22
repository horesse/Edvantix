using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Profiles;

public interface IProfileService
{
    Task<GetProfileResponse?> GetProfileByIdAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string id,
        CancellationToken cancellationToken = default
    );

    Task<GetProfilesResponse?> GetProfilesByIdsAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string[] ids,
        CancellationToken cancellationToken = default
    );

    /// <summary>Находит профиль по логину (preferred_username из Keycloak).</summary>
    Task<GetProfileByLoginResponse?> GetProfileByLoginAsync(
        string login,
        CancellationToken cancellationToken = default
    );
}
