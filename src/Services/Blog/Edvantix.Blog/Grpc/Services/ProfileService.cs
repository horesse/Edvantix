using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Grpc.Services;

[ExcludeFromCodeCoverage]
public class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service) : IProfileService
{
    public async Task<ulong> GetProfileIdByAccountId(
        Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileRequest() { AccountId = accountId.ToString() };
        var result = await service.GetProfileAsync(request, cancellationToken: cancellationToken);

        return result?.Id ?? throw new NotFoundException("Профиль не найден.");
    }

    public async Task<ProfileReply?> GetProfileById(
        ulong profileId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileRequest() { ProfileId = profileId };
        var result = await service.GetProfileAsync(request, cancellationToken: cancellationToken);

        return result;
    }
}
