using Edvantix.Organizational.Features.GroupFeature.Models;
using Edvantix.Organizational.Infrastructure.Services;
using Edvantix.SharedKernel.Results;

namespace Edvantix.Organizational.Features.GroupFeature.Features.GetOrganizationGroups;

public sealed record GetOrganizationGroupsQuery(
    long OrganizationId,
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
    public async Task<PagedResult<GroupModel>> Handle(
        GetOrganizationGroupsQuery request,
        CancellationToken cancellationToken
    )
    {
        var authService = provider.GetRequiredService<IOrganizationAuthorizationService>();
        await authService.GetCurrentMemberAsync(request.OrganizationId, cancellationToken);

        var spec = new GroupSpecification { OrganizationId = request.OrganizationId };

        using var groupRepo = provider.GetRequiredService<IGroupRepository>();

        var count = await groupRepo.GetCountByExpressionAsync(spec, cancellationToken);

        spec.Skip = (request.PageIndex - 1) * request.PageSize;
        spec.Take = request.PageSize;

        var groups = await groupRepo.GetByExpressionAsync(spec, cancellationToken);

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
