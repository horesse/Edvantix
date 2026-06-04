using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;
using Edvantix.Organizational.Grpc.Services.Groups;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.List;

/// <summary>Запрос постраничного списка кабинетов организации.</summary>
/// <param name="Search">Строка поиска по названию (опционально).</param>
/// <param name="IsArchive">Показать только архивные записи.</param>
/// <param name="Page">Номер страницы (от 1).</param>
/// <param name="PageSize">Размер страницы.</param>
[RequirePermission(OrganizationPermissions.View)]
public sealed record ListRoomsQuery(
    [property: Description("Строка поиска по названию")] string? Search = null,
    [property: Description("Показать только архивные записи")] bool IsArchive = false,
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

        var listSpec = new RoomListSpecification(
            orgId,
            query.IsArchive,
            query.Search,
            query.Page,
            query.PageSize
        );

        var countSpec = new RoomCountSpecification(orgId, query.IsArchive, query.Search);

        var items = await repository.ListAsync(listSpec, cancellationToken);
        var total = await repository.CountAsync(countSpec, cancellationToken);

        var counts = await groupsUsageService.CountByRoomIdsAsync(
            items.Select(r => r.Id),
            cancellationToken
        );

        var dtos = mapper
            .Map(items)
            .Select(dto =>
                dto with
                {
                    Usage = [new DirectoryUsageDto("Группы", counts.GetValueOrDefault(dto.Id, 0))],
                }
            )
            .ToList();

        return new PagedResult<RoomListItemDto>(dtos, query.Page, query.PageSize, total);
    }
}
