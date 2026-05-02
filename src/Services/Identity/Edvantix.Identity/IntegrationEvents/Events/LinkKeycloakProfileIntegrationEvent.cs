namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для привязки profileId к аккаунту Keycloak.
/// Публикуется Persona-сервисом при регистрации профиля;
/// потребляется Identity-сервисом через Wolverine.
/// </summary>
public sealed record LinkKeycloakProfileIntegrationEvent(Guid AccountId, Guid ProfileId)
    : IntegrationEvent;
