using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Features.OrganizationCustomRoleFeature;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature.Commands.CreateCustomRole;

/// <summary>
/// Команда создания новой кастомной роли в организации.
/// </summary>
public sealed record CreateCustomRoleCommand(
    Guid OrganizationId,
    string Code,
    OrganizationBaseRole BaseRole,
    string? Description
) : ICommand<Guid>;

/// <summary>
/// Обработчик команды создания кастомной роли.
/// Делегирует бизнес-логику в <see cref="IOrganizationCustomRoleService"/>.
/// </summary>
public sealed class CreateCustomRoleCommandHandler(IServiceProvider provider)
    : ICommandHandler<CreateCustomRoleCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        CreateCustomRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        var service = provider.GetRequiredService<IOrganizationCustomRoleService>();
        return await service.CreateAsync(
            request.OrganizationId,
            request.Code,
            request.BaseRole,
            request.Description,
            cancellationToken
        );
    }
}
