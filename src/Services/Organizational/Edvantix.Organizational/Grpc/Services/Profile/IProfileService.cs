using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Profile;

public interface IProfileService
{
    Task<ProfileResponse?> GetProfileByIdAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string id,
        CancellationToken cancellationToken = default
    );
}
