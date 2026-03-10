using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.PatchCustomRole;

/// <summary>
/// Команда частичного обновления кастомной роли.
/// Изменяет только базовую роль и описание — код остаётся неизменным.
/// </summary>
public sealed record PatchCustomRoleCommand(
    Guid RoleId,
    Guid OrganizationId,
    OrganizationBaseRole BaseRole,
    string? Description
) : ICommand<Unit>;

/// <summary>
/// Обработчик команды частичного обновления кастомной роли.
/// Делегирует бизнес-логику в <see cref="IOrganizationCustomRoleService"/>.
/// </summary>
public sealed class PatchCustomRoleCommandHandler(IServiceProvider provider)
    : ICommandHandler<PatchCustomRoleCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        PatchCustomRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        await service.PatchAsync(
            request.RoleId,
            request.OrganizationId,
            request.BaseRole,
            request.Description,
            cancellationToken
        );

        return Unit.Value;
    }
}
