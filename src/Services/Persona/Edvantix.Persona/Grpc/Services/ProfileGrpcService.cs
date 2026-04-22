using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.Persona.Grpc.Services;

internal sealed class ProfileService(IProfileRepository profileRepo)
    : ProfileGrpcService.ProfileGrpcServiceBase
{
    [AllowAnonymous]
    public override async Task<GetProfileResponse> GetProfile(
        GetProfileRequest request,
        ServerCallContext context
    )
    {
        var spec = ProfileSpecification.Minimal(Guid.Parse(request.ProfileId));

        var profile = await profileRepo.FindAsync(spec, context.CancellationToken);

        if (profile is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Профиль не найден."));

        return MapToResponse(profile);
    }

    [Authorize]
    [EnableRateLimiting("PerUserRateLimit")]
    public override async Task<GetProfilesResponse> GetProfiles(
        GetProfilesRequest request,
        ServerCallContext context
    )
    {
        var profiles = await profileRepo.ListAsync(
            new ProfileSpecification([.. request.ProfileIds.Select(Guid.Parse)]),
            context.CancellationToken
        );

        return MapToResponse(profiles);
    }

    private static GetProfilesResponse MapToResponse(IReadOnlyCollection<Profile> profile)
    {
        var response = new GetProfilesResponse();

        response.Profiles.AddRange(profile.Select(MapToResponse));
        return response;
    }

    [AllowAnonymous]
    public override async Task<GetProfileByLoginResponse> GetProfileByLogin(
        GetProfileByLoginRequest request,
        ServerCallContext context
    )
    {
        var spec = ProfileSpecification.ForLogin(request.Login);
        var profile = await profileRepo.FindAsync(spec, context.CancellationToken);

        if (profile is null)
            throw new RpcException(new Status(StatusCode.NotFound, "Профиль не найден."));

        return new GetProfileByLoginResponse
        {
            ProfileId = profile.Id.ToString(),
            AccountId = profile.AccountId.ToString(),
            FullName = profile.FullName.GetFullName(),
            Login = profile.Login,
        };
    }

    private static GetProfileResponse MapToResponse(Profile profile)
    {
        return new GetProfileResponse
        {
            Id = profile.Id.ToString(),
            FullName = profile.FullName.GetFullName(),
            FirstName = profile.FullName.FirstName,
            LastName = profile.FullName.LastName,
            MiddleName = profile.FullName.MiddleName ?? string.Empty,
        };
    }
}
