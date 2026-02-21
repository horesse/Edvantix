namespace Edvantix.Organizational.Grpc.Services;

[ExcludeFromCodeCoverage]
public class ProfileService(ProfileGrpcService.ProfileGrpcServiceClient service) : IProfileService
{
    public async Task<ulong> GetProfileIdByAccountId(
        Guid accountId,
        CancellationToken cancellationToken
    )
    {
        var request = new GetProfileByAccountIdRequest { AccountId = accountId.ToString() };
        var result = await service.GetProfileByAccountIdAsync(
            request,
            cancellationToken: cancellationToken
        );

        return result?.Id ?? throw new NotFoundException("Профиль не найден.");
    }
}
