namespace Edvantix.Persona.Features.Admin.Profiles.List;

public sealed record GetAdminProfilesQuery(
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Поиск по имени или логину")] string? Search = null,
    [property: Description("Фильтр по статусу блокировки")] bool? IsBlocked = null
) : IQuery<PagedResult<AdminProfileDto>>;

public sealed class GetAdminProfilesQueryHandler(
    IProfileRepository repository,
    IMapper<Profile, AdminProfileDto> mapper
) : IQueryHandler<GetAdminProfilesQuery, PagedResult<AdminProfileDto>>
{
    public async ValueTask<PagedResult<AdminProfileDto>> Handle(
        GetAdminProfilesQuery request,
        CancellationToken cancellationToken
    )
    {
        var clamped = (
            PageIndex: Math.Max(request.PageIndex, 1),
            PageSize: Math.Clamp(request.PageSize, 1, 100)
        );

        var offset = (clamped.PageIndex - 1) * clamped.PageSize;

        var listSpec = new AdminProfileListSpecification(
            offset,
            clamped.PageSize,
            request.Search,
            request.IsBlocked
        );

        var countSpec = new AdminProfileCountSpecification(request.Search, request.IsBlocked);

        var profiles = await repository.ListAsync(listSpec, cancellationToken);
        var totalCount = await repository.CountAsync(countSpec, cancellationToken);

        var items = profiles.Select(mapper.Map).ToList();

        return new PagedResult<AdminProfileDto>(
            items,
            clamped.PageIndex,
            clamped.PageSize,
            totalCount
        );
    }
}
