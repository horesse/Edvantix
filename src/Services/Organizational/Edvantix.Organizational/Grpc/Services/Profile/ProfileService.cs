using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Profile;

[ExcludeFromCodeCoverage]
internal sealed class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service)
    : IProfileService
{
    public async Task<ProfileResponse?> GetProfileByIdAsync(
        [StringSyntax(StringSyntaxAttribute.GuidFormat)] string id,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.GetProfileAsync(
            new GetProfileRequest { ProfileId = id },
            cancellationToken: cancellationToken
        );

        return result;
    }
}
