namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

/// <summary>
/// Кастомная роль организации, определяющая специфические права доступа
/// на основе одной из базовых ролей.
/// </summary>
public sealed class OrganizationCustomRole() : Entity<Guid>, IAggregateRoot, ISoftDelete
{
    /// <summary>
    /// Создаёт новую кастомную роль для указанной организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации-владельца.</param>
    /// <param name="code">Уникальный код роли в рамках организации (до 50 символов).</param>
    /// <param name="baseRole">Базовая роль, определяющая уровень доступа.</param>
    /// <param name="description">Описание роли и её полномочий (до 100 символов).</param>
    public OrganizationCustomRole(
        Guid organizationId,
        string code,
        OrganizationBaseRole baseRole,
        string? description = null
    )
        : this()
    {
        OrganizationId = organizationId;
        Code = code;
        BaseRole = baseRole;
        Description = description;
        IsDeleted = false;
    }

    /// <summary>Идентификатор организации, которой принадлежит роль.</summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>Ссылка на организацию.</summary>
    public Organization Organization { get; private set; } = null!;

    /// <summary>Уникальный код роли в рамках организации.</summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>Описание роли и её полномочий.</summary>
    public string? Description { get; private set; }

    /// <summary>Базовая роль, определяющая уровень доступа.</summary>
    public OrganizationBaseRole BaseRole { get; private set; }

    /// <summary>Флаг мягкого удаления.</summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Обновляет код кастомной роли.
    /// </summary>
    /// <param name="code">Новый уникальный код роли (до 50 символов).</param>
    /// <exception cref="InvalidOperationException">Если роль уже удалена.</exception>
    public void UpdateCode(string code)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Нельзя изменить код удалённой роли.");
        }

        Code = code;
    }

    /// <summary>
    /// Обновляет базовую роль, определяющую уровень доступа.
    /// </summary>
    /// <param name="baseRole">Новая базовая роль.</param>
    /// <exception cref="InvalidOperationException">Если роль уже удалена.</exception>
    public void UpdateBaseRole(OrganizationBaseRole baseRole)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Нельзя изменить базовую роль удалённой роли.");
        }

        BaseRole = baseRole;
    }

    /// <summary>
    /// Обновляет описание кастомной роли.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        Description = description;
    }

    /// <summary>
    /// Помечает роль как удалённую (мягкое удаление).
    /// </summary>
    /// <exception cref="InvalidOperationException">Если роль уже удалена.</exception>
    public void Delete()
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException("Кастомная роль организации уже удалена.");
        }

        IsDeleted = true;
    }
}
