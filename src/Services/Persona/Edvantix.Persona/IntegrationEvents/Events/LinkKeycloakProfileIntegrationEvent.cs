namespace Edvantix.Contracts;

/// <summary>
/// Интеграционное событие для привязки profileId к аккаунту Keycloak.
/// Публикуется при регистрации профиля; потребляется Identity-сервисом.
/// </summary>
public sealed record LinkKeycloakProfileIntegrationEvent(Guid AccountId, Guid ProfileId)
    : IntegrationEvent;
