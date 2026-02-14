using Edvantix.Chassis.Exceptions;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Company.Domain.AggregatesModel.GroupAggregate.Specifications;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Company.Domain.AggregatesModel.OrganizationMemberAggregate.Specifications;
using Edvantix.Company.Grpc.Services;

namespace Edvantix.Company.Services;

/// <summary>
/// Реализация сервиса авторизации на уровне организации и групп.
/// </summary>
public sealed class OrganizationAuthorizationService(IServiceProvider provider)
    : IOrganizationAuthorizationService
{
    /// <inheritdoc />
    public async Task<OrganizationMember> GetCurrentMemberAsync(
        long organizationId,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new OrganizationMemberByProfileSpecification(profileId, organizationId);

        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var member = await memberRepo.GetFirstByExpressionAsync(spec, cancellationToken) ?? throw new ForbiddenException(
                "Вы не являетесь участником данной организации."
            );
        
        return member;
    }

    /// <inheritdoc />
    public async Task<OrganizationMember> RequireOrgRoleAsync(
        long organizationId,
        CancellationToken cancellationToken,
        params OrganizationRole[] allowedRoles
    )
    {
        var member = await GetCurrentMemberAsync(organizationId, cancellationToken);

        if (!allowedRoles.Contains(member.Role))
            throw new ForbiddenException(
                "У вас недостаточно прав для выполнения данного действия."
            );

        return member;
    }

    /// <inheritdoc />
    public async Task RequireGroupManagementAsync(
        long groupId,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        // Получить группу для определения организации
        using var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group = await groupRepo.GetByIdAsync(groupId, cancellationToken);

        if (group is null)
            throw new NotFoundException($"Группа с ID {groupId} не найдена.");

        // Проверить роль на уровне организации
        var orgSpec = new OrganizationMemberByProfileSpecification(profileId, group.OrganizationId);
        using var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var orgMember = await memberRepo.GetFirstByExpressionAsync(orgSpec, cancellationToken);

        if (orgMember is null)
            throw new ForbiddenException(
                "Вы не являетесь участником организации, к которой принадлежит группа."
            );

        // Owner и Manager могут управлять любой группой
        if (orgMember.Role is OrganizationRole.Owner or OrganizationRole.Manager)
            return;

        // Проверить роль на уровне группы (Teacher или Manager группы)
        var groupMemberSpec = new GroupMemberByProfileSpecification(profileId, groupId);
        using var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var groupMember = await groupMemberRepo.GetFirstByExpressionAsync(
            groupMemberSpec,
            cancellationToken
        );

        if (groupMember?.Role is not (GroupRole.Teacher or GroupRole.Manager))
        {
            throw new ForbiddenException(
                "У вас недостаточно прав для управления данной группой."
            );
        }
    }
}
