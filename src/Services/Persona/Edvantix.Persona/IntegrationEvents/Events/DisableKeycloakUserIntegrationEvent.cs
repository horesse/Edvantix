namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для блокировки учётной записи Keycloak.
/// Публикуется при блокировке профиля; потребляется Identity-сервисом.
/// </summary>
public sealed record DisableKeycloakUserIntegrationEvent(Guid AccountId) : IntegrationEvent;
