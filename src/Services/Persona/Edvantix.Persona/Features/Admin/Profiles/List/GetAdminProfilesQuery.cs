using Edvantix.Persona.Features.Admin.Profiles;

namespace Edvantix.Persona.Features.Admin.Profiles.List;

public sealed record GetAdminProfilesQuery(
    [property: DefaultValue(1)] int PageIndex = 1,
    [property: DefaultValue(20)] int PageSize = 20,
    string? Search = null,
    bool? IsBlocked = null
) : IQuery<AdminProfilesResponse>;

public sealed class GetAdminProfilesQueryHandler(
    IProfileRepository repository,
    IMapper<Profile, AdminProfileDto> mapper
) : IQueryHandler<GetAdminProfilesQuery, AdminProfilesResponse>
{
    public async ValueTask<AdminProfilesResponse> Handle(
        GetAdminProfilesQuery request,
        CancellationToken cancellationToken
    )
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var offset = (request.PageIndex - 1) * pageSize;

        var listSpec = new AdminProfileListSpecification(
            offset,
            pageSize,
            request.Search,
            request.IsBlocked
        );

        var countSpec = new AdminProfileCountSpecification(
            request.Search,
            request.IsBlocked
        );

        var profiles = await repository.FindAllAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = profiles.Select(mapper.Map).ToList();

        return new AdminProfilesResponse(items, totalCount);
    }
}
