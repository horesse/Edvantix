using Edvantix.Organizational.Features.GroupFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Organizational.Features.GroupFeature.Features.GetOrganizationGroups;

public sealed record GetOrganizationGroupsQuery(
    Guid OrganizationId,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description(
        "Количество элементов, которые должны быть отображены на одной странице результатов."
    )]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Свойство для упорядочивания результатов")] string? OrderBy = null,
    [property: Description("При выборе порядка сортировки результат будет в порядке убывания.")]
    [property: DefaultValue(false)]
        bool IsDescending = false
) : IRequest<PagedResult<GroupModel>>;

public sealed class GetOrganizationGroupsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetOrganizationGroupsQuery, PagedResult<GroupModel>>
{
    public async ValueTask<PagedResult<GroupModel>> Handle(
        GetOrganizationGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        // Spec с includeMembers для корректного подсчёта MembersCount
        var spec = new GroupSpecification(request.OrganizationId, includeMembers: true);

        var groupRepo = provider.GetRequiredService<IGroupRepository>();

        var count = await groupRepo.CountAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var groups = await groupRepo.ListAsync(spec, cancellationToken);

        var items = groups
            .Select(g => new GroupModel
            {
                Id = g.Id,
                OrganizationId = g.OrganizationId,
                Name = g.Name,
                Description = g.Description,
                MembersCount = g.Members.Count,
            })
            .ToList();

        return new PagedResult<GroupModel>(items, request.PageIndex, request.PageSize, count);
    }
}
