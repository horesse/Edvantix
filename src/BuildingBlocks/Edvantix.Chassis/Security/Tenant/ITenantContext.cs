namespace Edvantix.Chassis.Security.Tenant;

/// <summary>
/// Представляет текущий контекст арендатора (организации) для запроса.
/// Определяется с помощью <see cref="TenantMiddleware"/> из HTTP-заголовка <c>X-OrganizationId</c>.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Получает определённый идентификатор организации для текущего запроса.
    /// </summary>
    /// <exception cref="InvalidOperationException">Выбрасывается, если контекст не был определён.</exception>
    Guid OrganizationId { get; }

    /// <summary>
    /// Получает значение, указывающее, был ли определён контекст арендатора.
    /// </summary>
    bool IsResolved { get; }

    /// <summary>
    /// Определяет контекст арендатора с заданным идентификатором организации.
    /// </summary>
    /// <param name="organizationId">Идентификатор организации, извлеченный из заголовка запроса.</param>
    void Resolve(Guid organizationId);
}
