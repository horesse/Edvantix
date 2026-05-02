namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для разблокировки учётной записи Keycloak.
/// Публикуется при снятии блокировки профиля; потребляется Identity-сервисом.
/// </summary>
public sealed record EnableKeycloakUserIntegrationEvent(Guid AccountId) : IntegrationEvent;
