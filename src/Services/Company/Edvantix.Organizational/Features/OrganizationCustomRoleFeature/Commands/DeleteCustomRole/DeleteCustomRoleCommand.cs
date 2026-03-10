using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.DeleteCustomRole;

/// <summary>
/// Команда мягкого удаления кастомной роли организации.
/// </summary>
public sealed record DeleteCustomRoleCommand(Guid RoleId, Guid OrganizationId) : ICommand<Unit>;

/// <summary>
/// Обработчик команды удаления кастомной роли.
/// Делегирует бизнес-логику в <see cref="IOrganizationCustomRoleService"/>.
/// </summary>
public sealed class DeleteCustomRoleCommandHandler(IServiceProvider provider)
    : ICommandHandler<DeleteCustomRoleCommand, Unit>
{
    public async ValueTask<Unit> Handle(
        DeleteCustomRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        await service.DeleteAsync(request.RoleId, request.OrganizationId, cancellationToken);

        return Unit.Value;
    }
}
