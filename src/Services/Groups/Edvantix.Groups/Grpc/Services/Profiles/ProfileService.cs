using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Groups.Grpc.Services.Profiles;

[ExcludeFromCodeCoverage]
internal sealed class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service)
    : IProfileService
{
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
