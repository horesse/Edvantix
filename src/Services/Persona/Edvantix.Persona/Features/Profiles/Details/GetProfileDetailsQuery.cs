namespace Edvantix.Persona.Features.Profiles.Details;

public sealed record GetProfileDetailsQuery() : IQuery<ProfileDetailsDto>;

public sealed class GetProfileDetailsQueryHandler(
    IProfileRepository repository,
    ClaimsPrincipal claims,
    IMapper<Profile, ProfileDetailsDto> mapper
) : IQueryHandler<GetProfileDetailsQuery, ProfileDetailsDto>
{
    public async ValueTask<ProfileDetailsDto> Handle(
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
