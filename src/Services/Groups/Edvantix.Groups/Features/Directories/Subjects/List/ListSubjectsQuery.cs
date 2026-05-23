using Edvantix.Chassis.CQRS;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate;
using Edvantix.Groups.Domain.AggregatesModel.SubjectAggregate.Specifications;
using Edvantix.Permissions;

namespace Edvantix.Groups.Features.Directories.Subjects.List;

/// <summary>
/// Возвращает постраничный список предметов организации.
/// </summary>
/// <param name="Search">Текстовый поиск по названию.</param>
/// <param name="IncludeArchived">Включить архивные записи.</param>
/// <param name="Page">Номер страницы (начиная с 1).</param>
/// <param name="Size">Размер страницы.</param>
[RequirePermission(SubjectPermissions.View)]
public sealed record ListSubjectsQuery(
    [property: Description("Текстовый поиск по названию")] string? Search = null,
    [property: Description("Включить архивные записи")] bool IncludeArchived = false,
    [property: DefaultValue(Pagination.DefaultPageIndex)] int Page = Pagination.DefaultPageIndex,
    [property: DefaultValue(Pagination.DefaultPageSize)] int Size = Pagination.DefaultPageSize
) : IQuery<PagedResult<SubjectListItemDto>>;

internal sealed class ListSubjectsQueryHandler(
    ITenantContext tenantContext,
    ISubjectRepository repository,
    IMapper<Subject, SubjectListItemDto> mapper
) : IQueryHandler<ListSubjectsQuery, PagedResult<SubjectListItemDto>>
{
    public async ValueTask<PagedResult<SubjectListItemDto>> Handle(
        ListSubjectsQuery request,
        CancellationToken cancellationToken
    )
    {
        var page = Math.Max(request.Page, 1);
        var size = Math.Clamp(request.Size, 1, 100);
        var offset = (page - 1) * size;
        var orgId = tenantContext.OrganizationId;

        var listSpec = new SubjectListSpec(orgId, offset, size, request.Search, request.IncludeArchived);
        var countSpec = new SubjectListSpec(orgId, request.Search, request.IncludeArchived);

        var subjects = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var items = subjects.Select(mapper.Map).ToList();

        return new PagedResult<SubjectListItemDto>(items, page, size, total);
    }
}
