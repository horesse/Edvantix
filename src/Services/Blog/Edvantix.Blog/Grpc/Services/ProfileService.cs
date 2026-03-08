using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Grpc.Services;

[ExcludeFromCodeCoverage]
public class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service) : IProfileService
{
    public async Task<ProfileReply?> GetProfileById(
        Guid profileId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileRequest { ProfileId = profileId.ToString() };
        var result = await service.GetProfileAsync(request, cancellationToken: cancellationToken);

        return result;
    }
}
