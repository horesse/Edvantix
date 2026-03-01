namespace Edvantix.Persona.Domain.Events;

/// <summary>
/// Доменное событие, публикуемое при создании нового профиля пользователя.
/// Используется для нотификации других сервисов через шину событий.
/// </summary>
public sealed class ProfileRegisteredEvent(Guid accountId, string login) : DomainEvent
{
    public Guid AccountId { get; } = accountId;
    public string Login { get; } = login;
}
