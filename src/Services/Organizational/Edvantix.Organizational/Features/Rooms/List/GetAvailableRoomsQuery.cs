using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Rooms.List;

/// <summary>
/// Возвращает список доступных (не удалённых) кабинетов организации,
/// отсортированных по принципу: подходящие по вместимости — первыми, затем слишком маленькие.
/// </summary>
/// <param name="MinCapacity">
/// Минимальная требуемая вместимость. Если указана, кабинеты с меньшим числом мест
/// перемещаются в конец списка, а <see cref="RoomDto.FitsTight"/> выставляется
/// для кабинетов с запасом менее 30%.
/// </param>
[RequirePermission(OrganizationPermissions.Groups)]
public sealed record GetAvailableRoomsQuery(
    [property: Description("Минимальная требуемая вместимость для фильтрации")]
        short? MinCapacity = null
) : IQuery<IReadOnlyList<RoomDto>>;

internal sealed class GetAvailableRoomsQueryHandler(
    ITenantContext tenantContext,
    IRoomRepository repository,
    IMapper<Room, RoomDto> mapper
) : IQueryHandler<GetAvailableRoomsQuery, IReadOnlyList<RoomDto>>
{
    public async ValueTask<IReadOnlyList<RoomDto>> Handle(
        GetAvailableRoomsQuery request,
        CancellationToken cancellationToken
    )
    {
        var rooms = await repository.ListByOrganizationAsync(
            tenantContext.OrganizationId,
            cancellationToken
        );

        var minCapacity = request.MinCapacity;

        return rooms
            .Select(r =>
            {
                var fits = minCapacity is null || r.Seats >= minCapacity.Value;
                // Запас менее 30% — кабинет «тесный», предупреждаем пользователя
                var fitsTight =
                    fits
                    && minCapacity is not null
                    && r.Seats < (int)Math.Ceiling(minCapacity.Value * 1.3);

                return mapper.Map(r) with { FitsTight = fitsTight };
            })
            .OrderByDescending(r => minCapacity is null || r.Seats >= minCapacity.Value)
            .ThenBy(r => r.Seats)
            .ToList();
    }
}
