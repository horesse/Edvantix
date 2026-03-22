using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Persona.Features.Profiles.Details;

public sealed record GetProfileDetailsQuery() : IQuery<ProfileDetailsModel>;

public sealed class GetProfileDetailsQueryHandler(
    IProfileRepository repository,
    ClaimsPrincipal claims,
    IMapper<Profile, ProfileDetailsModel> mapper
) : IQueryHandler<GetProfileDetailsQuery, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        GetProfileDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.GetProfileIdOrError();

        var spec = ProfileSpecification.ForRead(profileId);

        var profile = await repository.FindAsync(spec, cancellationToken);
        Guard.Against.NotFound(profile, profileId);

        return mapper.Map(profile);
    }
}
