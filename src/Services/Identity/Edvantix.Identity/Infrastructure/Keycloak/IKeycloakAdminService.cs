namespace Edvantix.Identity.Infrastructure.Keycloak;

/// <summary>Сервис для взаимодействия с Keycloak Admin REST API.</summary>
public interface IKeycloakAdminService
{
    /// <summary>
    /// Сохраняет <paramref name="profileId"/> как пользовательский атрибут
    /// в учётной записи Keycloak с идентификатором <paramref name="accountId"/>.
    /// </summary>
    Task SetProfileIdAsync(
        Guid accountId,
        Guid profileId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Отключает учётную запись Keycloak (блокировка пользователя).</summary>
    Task DisableUserAsync(Guid accountId, CancellationToken cancellationToken = default);

    /// <summary>Включает учётную запись Keycloak (снятие блокировки пользователя).</summary>
    Task EnableUserAsync(Guid accountId, CancellationToken cancellationToken = default);
}
