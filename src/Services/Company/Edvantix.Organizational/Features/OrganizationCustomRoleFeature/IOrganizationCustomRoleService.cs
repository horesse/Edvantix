using Edvantix.Organizational.Domain.AggregatesModel.OrganizationCustomRoleAggregate;

namespace Edvantix.Organizational.Features.OrganizationCustomRoleFeature;

/// <summary>
/// Сервис управления кастомными ролями организации.
/// Содержит бизнес-логику CRUD-операций с проверкой прав и инвариантов домена.
/// </summary>
public interface IOrganizationCustomRoleService
{
    /// <summary>
    /// Создаёт новую кастомную роль в организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="code">Уникальный код роли (до 50 символов).</param>
    /// <param name="baseRole">Базовая роль для наследования прав доступа.</param>
    /// <param name="description">Описание роли (до 100 символов, необязательно).</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Идентификатор созданной роли.</returns>
    /// <exception cref="ForbiddenException">Если инициатор не является Owner организации.</exception>
    /// <exception cref="InvalidOperationException">Если роль с таким кодом уже существует.</exception>
    Task<Guid> CreateAsync(
        Guid organizationId,
        string code,
        OrganizationBaseRole baseRole,
        string? description,
        CancellationToken ct = default
    );

    /// <summary>
    /// Обновляет существующую кастомную роль.
    /// </summary>
    /// <param name="roleId">Идентификатор роли.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="code">Новый код роли.</param>
    /// <param name="baseRole">Новая базовая роль.</param>
    /// <param name="description">Новое описание (null — очищает описание).</param>
    /// <param name="ct">Токен отмены.</param>
    /// <exception cref="ForbiddenException">Если инициатор не является Owner организации.</exception>
    /// <exception cref="NotFoundException">Если роль не найдена.</exception>
    /// <exception cref="InvalidOperationException">
    /// Если изменение кода невозможно (роль назначена пользователям или код занят).
    /// </exception>
    Task UpdateAsync(
        Guid roleId,
        Guid organizationId,
        string code,
        OrganizationBaseRole baseRole,
        string? description,
        CancellationToken ct = default
    );

    /// <summary>
    /// Мягко удаляет кастомную роль организации.
    /// </summary>
    /// <param name="roleId">Идентификатор роли.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <exception cref="ForbiddenException">Если инициатор не является Owner организации.</exception>
    /// <exception cref="NotFoundException">Если роль не найдена.</exception>
    /// <exception cref="InvalidOperationException">
    /// Если роль назначена активным пользователям.
    /// </exception>
    Task DeleteAsync(Guid roleId, Guid organizationId, CancellationToken ct = default);

    /// <summary>
    /// Возвращает список активных кастомных ролей организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Список активных (не удалённых) кастомных ролей.</returns>
    /// <exception cref="ForbiddenException">Если инициатор не является участником организации.</exception>
    Task<IReadOnlyList<OrganizationCustomRole>> ListAsync(
        Guid organizationId,
        CancellationToken ct = default
    );

    /// <summary>
    /// Возвращает кастомную роль по идентификатору.
    /// </summary>
    /// <param name="roleId">Идентификатор роли.</param>
    /// <param name="organizationId">Идентификатор организации.</param>
    /// <param name="ct">Токен отмены.</param>
    /// <exception cref="ForbiddenException">Если инициатор не является участником организации.</exception>
    /// <exception cref="NotFoundException">Если роль не найдена.</exception>
    Task<OrganizationCustomRole> GetByIdAsync(
        Guid roleId,
        Guid organizationId,
        CancellationToken ct = default
    );
}
