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
}
