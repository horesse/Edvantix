using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.Events;

namespace Edvantix.Organizational.Domain.EventHandlers;

/// <summary>
/// Создаёт базовую матрицу ролей и назначает владельца организации.
/// Выполняется в той же транзакционной области после сохранения агрегата Organization.
/// </summary>
internal sealed class OrganizationCreatedDomainEventHandler(
    IOrganizationMemberRoleRepository memberRoleRepository,
    IGroupRoleRepository groupRoleRepository,
    IOrganizationMemberRepository memberRepository,
    IPermissionRepository permissionRepository
) : INotificationHandler<OrganizationCreatedDomainEvent>
{
    public async ValueTask Handle(
        OrganizationCreatedDomainEvent notification,
        CancellationToken cancellationToken
    )
    {
        var orgPermissions = await permissionRepository.ListAsync(
            new PermissionByFeatureSpecification(OrganizationPermissions.Feature, true),
            cancellationToken
        );
        var groupPermissions = await permissionRepository.ListAsync(
            new PermissionByFeatureSpecification(GroupPermissions.Feature, true),
            cancellationToken
        );

        var (orgRoles, groupRoles) = OrganizationDefaultRolesFactory.CreateFor(
            notification.OrganizationId,
            [.. orgPermissions, .. groupPermissions]
        );

        await memberRoleRepository.AddRangeAsync(orgRoles, cancellationToken);
        await groupRoleRepository.AddRangeAsync(groupRoles, cancellationToken);

        var ownerRole = orgRoles.First(r => r.Code == "owner");
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
