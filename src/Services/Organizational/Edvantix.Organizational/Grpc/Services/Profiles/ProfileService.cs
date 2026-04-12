using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services.Profiles;

[ExcludeFromCodeCoverage]
internal sealed class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service)
    : IProfileService
{
    public async Task<GetProfileResponse?> GetProfileByIdAsync(
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

    public async Task<GetProfilesResponse?> GetProfilesByIdsAsync(
        string[] ids,
        CancellationToken cancellationToken = default
    )
    {
        var result = await service.GetProfilesAsync(
            new GetProfilesRequest { ProfileIds = { ids } },
            cancellationToken: cancellationToken
        );

        return result;
    }
}
