using Edvantix.Chassis.CQRS;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Permissions;

namespace Edvantix.Organizational.Features.Directories.Rooms.Create;

/// <summary>Запрос на создание кабинета в справочнике организации.</summary>
/// <param name="Name">Название кабинета.</param>
/// <param name="Capacity">Вместимость (1–1000).</param>
/// <param name="Floor">Номер/название этажа (до 10 символов); <c>null</c> — не указан.</param>
/// <param name="RoomType">Тип помещения.</param>
/// <param name="Order">Порядок сортировки.</param>
[Transactional]
[RequirePermission(OrganizationPermissions.Edit)]
public sealed record CreateRoomCommand(
    string Name,
    int Capacity,
    string? Floor,
    RoomType RoomType,
    int Order = 0
) : ICommand<RoomDto>;

internal sealed class CreateRoomCommandHandler(
    ITenantContext tenantContext,
    ClaimsPrincipal claims,
    IRoomRepository repository,
    IMapper<Room, RoomDto> mapper
) : ICommandHandler<CreateRoomCommand, RoomDto>
{
    public async ValueTask<RoomDto> Handle(
        CreateRoomCommand command,
        CancellationToken cancellationToken
    )
    {
        var createdBy = claims.GetProfileIdOrError();

        var room = new Room(
            tenantContext.OrganizationId,
            command.Name,
            command.Capacity,
            command.Floor,
            command.RoomType,
            command.Order,
            createdBy
        );

        await repository.AddAsync(room, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return mapper.Map(room);
    }
}
