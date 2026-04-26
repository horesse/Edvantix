namespace Edvantix.Persona.Features.Admin.Profiles.Get;

public sealed record GetAdminProfileQuery(Guid ProfileId) : IQuery<AdminProfileDetailDto>;

public sealed class GetAdminProfileQueryHandler(
    IProfileRepository repository,
    IMapper<Profile, AdminProfileDetailDto> mapper
) : IQueryHandler<GetAdminProfileQuery, AdminProfileDetailDto>
{
    public async ValueTask<AdminProfileDetailDto> Handle(
        GetAdminProfileQuery request,
        CancellationToken cancellationToken
    )
    {
        var spec = ProfileSpecification.ForRead(request.ProfileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, request.ProfileId);

        return mapper.Map(profile);
    }
}
