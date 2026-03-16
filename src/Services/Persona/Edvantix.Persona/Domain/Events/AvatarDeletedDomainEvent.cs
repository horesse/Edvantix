namespace Edvantix.Persona.Domain.Events;

/// <summary>
/// Доменное событие, публикуемое когда пользователь удаляет свой аватар.
/// Обрабатывается для удаления файла из хранилища после успешного сохранения профиля.
/// </summary>
public sealed class AvatarDeletedDomainEvent(string avatarUrn) : DomainEvent
{
    public string AvatarUrn { get; } = avatarUrn;
}
