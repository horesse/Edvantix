using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;
using Edvantix.Organizational.Domain.Permissions;
using Edvantix.Organizational.Grpc.Services.Groups;

namespace Edvantix.Organizational.Features.Directories.Rooms.List;

/// <summary>Запрос постраничного списка кабинетов организации.</summary>
/// <param name="Search">Строка поиска по названию (опционально).</param>
/// <param name="IncludeArchived">Включать ли архивные записи.</param>
/// <param name="Page">Номер страницы (от 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record ListRoomsQuery(
    [property: Description("Строка поиска по названию")] string? Search = null,
    [property: Description("Включать архивные записи")] bool IncludeArchived = false,
    [property: Description("Номер страницы (от 1)")] int Page = 1,
    [property: Description("Размер страницы")] int PageSize = 50
) : IQuery<PagedResult<RoomListItemDto>>;

internal sealed class ListRoomsQueryHandler(
    ITenantContext tenantContext,
    IRoomRepository repository,
    IMapper<Room, RoomListItemDto> mapper,
    IGroupsUsageService groupsUsageService
) : IQueryHandler<ListRoomsQuery, PagedResult<RoomListItemDto>>
{
    public async ValueTask<PagedResult<RoomListItemDto>> Handle(
        ListRoomsQuery query,
        CancellationToken cancellationToken
    )
    {
        var orgId = tenantContext.OrganizationId;

        // isArchived=null означает «все» (includeArchived=true), false — только активные
        var isArchivedFilter = query.IncludeArchived ? (bool?)null : false;

        var listSpec = new RoomListSpecification(
            orgId,
            query.IncludeArchived,
            query.Search,
            query.Page,
            query.PageSize
        );

        var countSpec = new RoomCountSpecification(orgId, isArchivedFilter, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var counts = await groupsUsageService.CountByRoomIdsAsync(
            items.Select(r => r.Id),
            cancellationToken
        );

        var dtos = mapper
            .Map(items)
            .Select(dto => dto with
            {
                Usage = [new DirectoryUsageDto("Группы", counts.GetValueOrDefault(dto.Id, 0))],
            })
            .ToList();

        return new PagedResult<RoomListItemDto>(dtos, query.Page, query.PageSize, total);
    }
}
