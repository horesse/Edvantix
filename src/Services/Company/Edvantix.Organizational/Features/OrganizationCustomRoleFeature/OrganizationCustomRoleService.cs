using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;
using Edvantix.Organizational.Infrastructure.Services;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature;

/// <summary>
/// Реализация сервиса управления кастомными ролями организации.
/// Содержит бизнес-логику CRUD-операций с проверкой прав и инвариантов домена.
/// </summary>
public sealed class OrganizationCustomRoleService(
    IOrganizationCustomRoleRepository roleRepo,
    IOrganizationAuthorizationService authService
) : IOrganizationCustomRoleService
{
    /// <inheritdoc/>
    public async Task<Guid> CreateAsync(
        Guid organizationId,
        string code,
        OrganizationBaseRole baseRole,
        string? description,
        CancellationToken ct = default
    )
    {
        // Только Owner может создавать кастомные роли.
        await authService.RequireOrgRoleAsync(organizationId, ct, OrganizationRole.Owner);

        // Код должен быть уникальным в рамках организации среди не удалённых ролей.
        var existing = await roleRepo.FindByCodeAsync(organizationId, code, ct);

        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"Кастомная роль с кодом '{code}' уже существует в организации."
            );
        }

        var role = new OrganizationCustomRole(organizationId, code, baseRole, description);
        await roleRepo.AddAsync(role, ct);
        await roleRepo.UnitOfWork.SaveEntitiesAsync(ct);

        return role.Id;
    }

    /// <inheritdoc/>
    public async Task UpdateAsync(
        Guid roleId,
        Guid organizationId,
        string code,
        OrganizationBaseRole baseRole,
        string? description,
        CancellationToken ct = default
    )
    {
        // Только Owner может изменять кастомные роли.
        await authService.RequireOrgRoleAsync(organizationId, ct, OrganizationRole.Owner);

        var role =
            await roleRepo.FindByIdAsync(roleId, organizationId, ct)
            ?? throw new NotFoundException(
                $"Кастомная роль с ID {roleId} не найдена в организации."
            );

        // Если код изменяется — проверяем дополнительные условия.
        if (!string.Equals(role.Code, code, StringComparison.OrdinalIgnoreCase))
        {
            // Нельзя менять код, если роль уже назначена пользователям.
            var assignedCount = await roleRepo.GetAssignedMembersCountAsync(roleId, ct);

            if (assignedCount > 0)
            {
                throw new InvalidOperationException(
                    $"Нельзя изменить код роли: роль назначена {assignedCount} пользователям."
                );
            }

            // Новый код должен быть уникальным в организации.
            var codeConflict = await roleRepo.FindByCodeAsync(organizationId, code, ct);

            if (codeConflict is not null)
            {
                throw new InvalidOperationException(
                    $"Кастомная роль с кодом '{code}' уже существует в организации."
                );
            }

            role.UpdateCode(code);
        }

        role.UpdateBaseRole(baseRole);
        role.UpdateDescription(description);

        await roleRepo.UnitOfWork.SaveEntitiesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(Guid roleId, Guid organizationId, CancellationToken ct = default)
    {
        // Только Owner может удалять кастомные роли.
        await authService.RequireOrgRoleAsync(organizationId, ct, OrganizationRole.Owner);

        var role =
            await roleRepo.FindByIdAsync(roleId, organizationId, ct)
            ?? throw new NotFoundException(
                $"Кастомная роль с ID {roleId} не найдена в организации."
            );

        // Нельзя удалить роль, если она назначена активным пользователям.
        var assignedCount = await roleRepo.GetAssignedMembersCountAsync(roleId, ct);

        if (assignedCount > 0)
        {
            throw new InvalidOperationException(
                $"Нельзя удалить роль: она назначена {assignedCount} пользователям. "
                    + "Сначала переназначьте пользователей на другую роль."
            );
        }

        role.Delete();
        await roleRepo.UnitOfWork.SaveEntitiesAsync(ct);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganizationCustomRole>> ListAsync(
        Guid organizationId,
        CancellationToken ct = default
    )
    {
        // Список ролей доступен всем участникам организации.
        await authService.GetCurrentMemberAsync(organizationId, ct);

        return await roleRepo.GetByOrganizationAsync(organizationId, ct);
    }

    /// <inheritdoc/>
    public async Task<OrganizationCustomRole> GetByIdAsync(
        Guid roleId,
        Guid organizationId,
        CancellationToken ct = default
    )
    {
        // Просмотр роли доступен всем участникам организации.
        await authService.GetCurrentMemberAsync(organizationId, ct);

        return await roleRepo.FindByIdAsync(roleId, organizationId, ct)
            ?? throw new NotFoundException(
                $"Кастомная роль с ID {roleId} не найдена в организации."
            );
    }
}
