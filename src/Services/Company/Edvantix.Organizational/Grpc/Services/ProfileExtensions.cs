using Edvantix.Chassis.Utilities;

namespace Edvantix.Organizational.Grpc.Services;

public static class ProfileExtensions
{
    public static async Task<ulong> GetProfileId(
        this IServiceProvider provider,
        CancellationToken cancellationToken
    )
    {
        var userId = provider.GetUserId();

        var profileService = provider.GetRequiredService<IProfileService>();
        var profileId = await profileService.GetProfileIdByAccountId(userId, cancellationToken);

        return profileId;
    }
}
