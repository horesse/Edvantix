namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для блокировки учётной записи Keycloak.
/// Публикуется Persona-сервисом при блокировке профиля администратором;
/// потребляется Identity-сервисом через MassTransit.
/// </summary>
public sealed record DisableKeycloakUserIntegrationEvent(Guid AccountId) : IntegrationEvent;
