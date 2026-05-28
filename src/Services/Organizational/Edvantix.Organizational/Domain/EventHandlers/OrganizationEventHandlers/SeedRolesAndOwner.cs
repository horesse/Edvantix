using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers.OrganizationEventHandlers;

/// <summary>
/// Засевает базовую матрицу ролей и назначает владельца организации.
/// Выполняется в той же транзакционной области после сохранения агрегата Organization.
/// </summary>
internal sealed class SeedRolesAndOwner(
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
        var organizationId = notification.OrganizationId;
        var permissions = await permissionRepository.GetAllAsync(cancellationToken);

        var roleData = new OrganizationRoleData(organizationId, permissions);
        await roleRepository.AddRangeAsync(roleData, cancellationToken);

        var ownerMember = new OrganizationMember(
            organizationId,
            notification.OwnerProfileId,
            roleData.OwnerRole.Id,
            DateOnly.FromDateTime(DateTime.UtcNow)
        );

        await memberRepository.AddAsync(ownerMember, cancellationToken);
        await memberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}
