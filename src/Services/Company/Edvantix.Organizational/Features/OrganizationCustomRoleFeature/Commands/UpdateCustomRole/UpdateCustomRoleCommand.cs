using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.UpdateCustomRole;

/// <summary>
/// Команда обновления кастомной роли организации.
/// </summary>
public sealed record UpdateCustomRoleCommand(
    Guid RoleId,
    Guid OrganizationId,
    string Code,
    OrganizationBaseRole BaseRole,
    string? Description
) : ICommand<Unit>;

/// <summary>
/// Обработчик команды обновления кастомной роли.
/// Делегирует бизнес-логику в <see cref="IOrganizationCustomRoleService"/>.
/// </summary>
public sealed class UpdateCustomRoleCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateCustomRoleCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        UpdateCustomRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        await service.UpdateAsync(
            request.RoleId,
            request.OrganizationId,
            request.Code,
            request.BaseRole,
            request.Description,
            cancellationToken
        );

        return Unit.Value;
    }
}
