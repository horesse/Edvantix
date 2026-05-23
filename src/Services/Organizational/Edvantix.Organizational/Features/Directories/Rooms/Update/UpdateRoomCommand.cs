using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.Update;

/// <summary>Запрос на обновление кабинета.</summary>
/// <param name="Id">Идентификатор записи (из маршрута).</param>
/// <param name="Name">Новое название.</param>
/// <param name="Capacity">Новая вместимость (1–1000).</param>
/// <param name="Floor">Новый номер/название этажа (до 10 символов); <c>null</c> — не указан.</param>
/// <param name="RoomType">Новый тип помещения.</param>
/// <param name="Order">Новый порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record UpdateRoomCommand(
    Guid Id,
    string Name,
    int Capacity,
    string? Floor,
    RoomType RoomType,
    int Order = 0
) : ICommand<RoomDto>;

internal sealed class UpdateRoomCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IRoomRepository repository,
    IMapper<Room, RoomDto> mapper
) : ICommandHandler<UpdateRoomCommand, RoomDto>
{
    public async ValueTask<RoomDto> Handle(
        UpdateRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var room = await repository.GetByIdAsync(command.Id, cancellationToken);

        if (room is null || room.OrganizationId != tenantContext.OrganizationId)
            throw NotFoundException.For<Room>(command.Id);

        var modifiedBy = claims.GetProfileIdOrError();

        room.Update(
            command.Name,
            command.Capacity,
            command.Floor,
            command.RoomType,
            command.Order,
            modifiedBy
        );

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(room);
    }
}
