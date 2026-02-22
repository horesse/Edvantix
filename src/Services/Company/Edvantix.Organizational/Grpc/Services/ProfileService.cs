using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.Grpc.Services;

[ExcludeFromCodeCoverage]
public class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service) : IProfileService
{
    public async Task<Guid> GetProfileIdByAccountId(
        Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileRequest() { AccountId = accountId.ToString() };
        var result = await service.GetProfileAsync(request, cancellationToken: cancellationToken);

        if (result is null)
            throw new NotFoundException("Профиль не найден.");

        return Guid.Parse(result.Id);
    }
}
