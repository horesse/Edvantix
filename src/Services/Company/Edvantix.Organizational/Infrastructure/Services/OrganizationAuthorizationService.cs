using Edvantix.Organizational.Grpc.Services;

namespace Edvantix.Organizational.Infrastructure.Services;

/// <summary>
/// Реализация сервиса авторизации на уровне организации и групп.
/// </summary>
public sealed class OrganizationAuthorizationService(IServiceProvider provider)
    : IOrganizationAuthorizationService
{
    /// <inheritdoc />
    public async Task<OrganizationMember> GetCurrentMemberAsync(
        ulong organizationId,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        var spec = new OrganizationMemberSpecification(profileId, organizationId);
        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();

        var member =
            await memberRepo.FindAsync(spec, cancellationToken)
            ?? throw new ForbiddenException("Вы не являетесь участником данной организации.");

        return member;
    }

    /// <inheritdoc />
    public async Task<OrganizationMember> RequireOrgRoleAsync(
        ulong organizationId,
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
        ulong groupId,
        CancellationToken cancellationToken
    )
    {
        var profileId = await provider.GetProfileId(cancellationToken);

        // Получить группу для определения организации
        var groupRepo = provider.GetRequiredService<IGroupRepository>();
        var group =
            await groupRepo.FindByIdAsync(groupId, cancellationToken)
            ?? throw new NotFoundException($"Группа с ID {groupId} не найдена.");

        // Проверить роль на уровне организации
        var orgSpec = new OrganizationMemberSpecification(profileId, group.OrganizationId);
        var memberRepo = provider.GetRequiredService<IOrganizationMemberRepository>();
        var orgMember = await memberRepo.FindAsync(orgSpec, cancellationToken);

        if (orgMember is null)
            throw new ForbiddenException(
                "Вы не являетесь участником организации, к которой принадлежит группа."
            );

        // Owner и Manager могут управлять любой группой
        if (orgMember.Role is OrganizationRole.Owner or OrganizationRole.Manager)
            return;

        // Проверить роль на уровне группы (Teacher или Manager группы)
        var groupMemberSpec = new GroupMemberSpecification(profileId, groupId);
        var groupMemberRepo = provider.GetRequiredService<IGroupMemberRepository>();
        var groupMember = await groupMemberRepo.FindAsync(groupMemberSpec, cancellationToken);

        if (groupMember?.Role is not (GroupRole.Teacher or GroupRole.Manager))
        {
            throw new ForbiddenException("У вас недостаточно прав для управления данной группой.");
        }
    }
}
