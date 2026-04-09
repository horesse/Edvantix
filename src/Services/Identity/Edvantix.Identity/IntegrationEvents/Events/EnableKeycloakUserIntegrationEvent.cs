namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для разблокировки учётной записи Keycloak.
/// Публикуется Persona-сервисом при снятии блокировки профиля;
/// потребляется Identity-сервисом через MassTransit.
/// </summary>
public sealed record EnableKeycloakUserIntegrationEvent(Guid AccountId) : IntegrationEvent;
