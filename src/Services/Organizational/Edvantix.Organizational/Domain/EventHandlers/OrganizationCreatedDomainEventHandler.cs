using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Создаёт базовую матрицу ролей и назначает владельца организации.
/// Выполняется в той же транзакционной области после сохранения агрегата Organization.
/// </summary>
internal sealed class OrganizationCreatedDomainEventHandler(
    IOrganizationRoleRepository roleRepository,
    IOrganizationMemberRepository memberRepository,
    IPermissionRepository permissionRepository
) : INotificationHandler<OrganizationCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var allPermissions = await permissionRepository.GetAllAsync(cancellationToken);

        var orgRoles = OrganizationDefaultRolesFactory.CreateFor(
            notification.OrganizationId,
            allPermissions
        );

        await roleRepository.AddRangeAsync(orgRoles, cancellationToken);

        var ownerRole = orgRoles.First(r => r.IsSystem);
        var ownerMember = new OrganizationMember(
            notification.OrganizationId,
            notification.OwnerProfileId,
            ownerRole.Id,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await memberRepository.AddAsync(ownerMember, cancellationToken);

        await memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
