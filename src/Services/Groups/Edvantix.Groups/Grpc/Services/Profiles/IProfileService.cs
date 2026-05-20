using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Groups.Grpc.Services.Profiles;

public interface IProfileService
{
    Task<GetProfilesResponse?> GetProfilesByIdsAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string[] ids,
        CancellationToken cancellationToken = default
    );
}
