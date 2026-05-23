using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.Rooms.Update;

internal sealed class UpdateRoomValidator : OrganizationScopedLookupValidator<UpdateRoomCommand>
{
    public UpdateRoomValidator(RoomUniqueNameChecker nameChecker, ITenantContext tenantContext)
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name, c => c.Id)
    {
        RuleFor(c => c.Capacity)
            .InclusiveBetween(Room.MinCapacity, Room.MaxCapacity)
            .WithMessage(
                $"Вместимость кабинета должна быть от {Room.MinCapacity} до {Room.MaxCapacity} мест."
            );

        RuleFor(c => c.Floor)
            .MaximumLength(Room.MaxFloorLength)
            .WithMessage($"Номер/название этажа не может превышать {Room.MaxFloorLength} символов.")
            .When(c => c.Floor is not null);

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок не может быть отрицательным.");
    }
}
